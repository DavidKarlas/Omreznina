using Omreznina.Models;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Omreznina.Client.Logic
{

    public record RawUsage15Min(DateTime DateTime, int Block, bool HighTariff, decimal ImportPower, decimal ImportEnergy, decimal ExportPower, decimal ExportEnergy);
    public record CalculatedUsage(RawUsage15Min Source, decimal OverdraftPower, decimal OverdraftPrice, decimal AgreedPowerPrice);

    public class OmrezninaReport : PeriodReport
    {
        public MonthlyReport[] MonthlyReports { get; }
        public CalculationOptions CalculationOptions { get; }

        public OmrezninaReport(CalculationOptions calculationOptions, Dictionary<(int Year, int Month), List<RawUsage15Min>>? rawUsages, ManuallyEnteredMonthlyConsumption[] manualEnergyUsageByMonths)
        {
            CalculationOptions = calculationOptions;
            if (rawUsages == null)
            {
                MonthlyReports = Enumerable.Range(1, 12).Select(m => new MonthlyReport(calculationOptions, (2023, m), [], manualEnergyUsageByMonths[m - 1])).ToArray();
            }
            else
            {
                MonthlyReports = rawUsages.Select(m => new MonthlyReport(calculationOptions, m.Key, m.Value, null)).OrderBy(m => m.Month).ToArray();
            }
            HighTariffImportEnergyInKWh = MonthlyReports.Sum(m => m.HighTariffImportEnergyInKWh);
            HighTariffExportEnergyInKWh = MonthlyReports.Sum(m => m.HighTariffExportEnergyInKWh);
            LowTariffImportEnergyInKWh = MonthlyReports.Sum(m => m.LowTariffImportEnergyInKWh);
            LowTariffExportEnergyInKWh = MonthlyReports.Sum(m => m.LowTariffExportEnergyInKWh);

            if (calculationOptions.SolarPowerPlant == "Imam" && calculationOptions.NetMetering)
            {
                var netMeteringEnergyInKWh = HighTariffImportEnergyInKWh
                    - HighTariffExportEnergyInKWh
                    + LowTariffImportEnergyInKWh
                    - LowTariffExportEnergyInKWh;
                netMeteringEnergyInKWh = Math.Max(netMeteringEnergyInKWh, 0);
                EnergyTransferPrice = netMeteringEnergyInKWh
                    * BlockPrices.GetNonBlockTransferEnergyPerKWH(null, calculationOptions);
                OldEnergyTransferPrice = netMeteringEnergyInKWh * BlockPrices.GetOldTransferEnergyPriceSingleTariffPerKWh(calculationOptions);
                if (calculationOptions.IncludeEnergyPrice)
                    EnergyPrice = netMeteringEnergyInKWh * calculationOptions.SingleTariffEnergyPrice * calculationOptions.VATMultiplier;
                else
                    EnergyPrice = 0;
            }
            else
            {
                EnergyTransferPrice = MonthlyReports.Sum(m => m.EnergyTransferPrice);
                OldEnergyTransferPrice = MonthlyReports.Sum(m => m.OldEnergyTransferPrice);
                EnergyPrice = MonthlyReports.Sum(m => m.EnergyPrice);
            }
            OldFixedPrice = MonthlyReports.Sum(m => m.OldFixedPrice);
            FixedPowerPriceIfNo15Minute = MonthlyReports.Sum(m => m.FixedPowerPriceIfNo15Minute);
            AgreedPowerPrice = MonthlyReports.Sum(m => m.AgreedPowerPrice);
            OverdraftPowerPrice = MonthlyReports.Sum(m => m.OverdraftPowerPrice);
            for (int i = 0; i < 5; i++)
            {
                OverdraftPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.OverdraftPowerPricePerBlock[i]);
                AgreedPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.AgreedPowerPricePerBlock[i]);
                EnergyImportPerBlockInKWh[i] = MonthlyReports.Sum(m => m.EnergyImportPerBlockInKWh[i]);
            }
        }

        public static async Task<Dictionary<(int Year, int Month), List<RawUsage15Min>>> ParseRawUsages(List<Stream> csvStreams)
        {
            var parsedUsageAll = new Dictionary<(int Year, int Month), List<RawUsage15Min>>();
            foreach (var stream in csvStreams)
            {
                using var streamReader = new StreamReader(stream);
                var csv = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                var dateTime15minColumn = csv.GetOrdinal("Časovna značka");
                var importPowerColumn = csv.GetOrdinal("P+ Prejeta delovna moč");
                var exportPowerColumn = csv.GetOrdinal("P- Oddana delovna moč");
                var importEnergyColumn = csv.GetOrdinal("Energija A+");
                var exportEnergyColumn = csv.GetOrdinal("Energija A-");
                var blockColumn = csv.GetOrdinal("Blok");
                while (csv.Read())
                {
                    // To avoid weirdness with 03-12-2023T00:00:00 actually belonging to 02-12-2023
                    // do here while parsing this offset so rest of code doesn't have to deal with this weirdness
                    var dateTime = csv.GetDateTime(dateTime15minColumn).AddMinutes(-15);
                    if (!parsedUsageAll.TryGetValue((dateTime.Year, dateTime.Month), out var parsedUsage))
                    {
                        parsedUsageAll[(dateTime.Year, dateTime.Month)] = parsedUsage = new List<RawUsage15Min>();
                    }
                    if (csv.GetFieldSpan(importPowerColumn).IsEmpty)
                        continue;
                    var importPower = decimal.Parse(csv.GetFieldSpan(importPowerColumn), CultureInfo.InvariantCulture);
                    var exportPower = csv.GetFieldSpan(exportPowerColumn).IsEmpty ? 0M : decimal.Parse(csv.GetFieldSpan(exportPowerColumn), CultureInfo.InvariantCulture);
                    var importEnergy = csv.GetDecimal(importEnergyColumn);
                    var exportEnergy = csv.GetFieldSpan(exportEnergyColumn).IsEmpty ? 0M : decimal.Parse(csv.GetFieldSpan(exportEnergyColumn), CultureInfo.InvariantCulture);
                    var theirBlock = csv.GetInt32(blockColumn) - 1;
                    //var myBlock = dateTime.ToBlock();
                    //if (myBlock != theirBlock)
                    //    Console.WriteLine($"My block:{myBlock} Their block:{theirBlock}");
                    parsedUsage.Add(new(dateTime, theirBlock, dateTime.IsHighTariff(), importPower, importEnergy, exportPower, exportEnergy));
                }
            }
            return parsedUsageAll;
        }
    }

    public class PeriodReport
    {
        #region Old prices
        public decimal OldFixedPrice { get; set; }
        public decimal OldEnergyTransferPrice { get; set; }
        public decimal OldEnergyTransferLowTariffPrice { get; set; }
        public decimal OldEnergyTransferHighTariffPrice { get; set; }
        #endregion

        public decimal EnergyPrice { get; set; }

        public decimal EnergyTransferPrice { get; set; }
        public decimal EnergyTransferLowTariffPrice { get; set; }
        public decimal EnergyTransferHighTariffPrice { get; set; }
        public decimal LowTariffImportEnergyInKWh { get; set; }
        public decimal LowTariffExportEnergyInKWh { get; set; }
        public decimal HighTariffImportEnergyInKWh { get; set; }
        public decimal HighTariffExportEnergyInKWh { get; set; }
        public decimal FixedPowerPriceIfNo15Minute { get; set; }
        public decimal AgreedPowerPrice { get; set; }
        public decimal OverdraftPowerPrice { get; set; }
        public decimal[] OverdraftPowerPricePerBlock { get; } = new decimal[5];
        public decimal[] AgreedPowerPricePerBlock { get; } = new decimal[5];
        public decimal[] EnergyImportPerBlockInKWh { get; } = new decimal[5];
    }

    public class DayReport : PeriodReport
    {
        public DateOnly Day { get; init; }
        public List<CalculatedUsage> Usages { get; } = new();
        public int Index { get; internal set; }
        public bool IsSelected { get; internal set; }
    }

    public class MonthlyReport : PeriodReport
    {
        public (int Year, int Month) Month { get; }
        public Dictionary<DateOnly, DayReport> DailyReports { get; } = new();

        public MonthlyReport(CalculationOptions calculationOptions, (int Year, int Month) month, List<RawUsage15Min> rawUsages, ManuallyEnteredMonthlyConsumption? manualMonthEnergy)
        {
            Month = month;
            for (int i = 0; i < 5; i++)
            {
                if (Month.Month < 3 || Month.Month > 10)
                {
                    if (i == 4)
                        continue;
                }
                else
                {
                    if (i == 0)
                        continue;
                }
                if (!calculationOptions.Has15MinuteData)
                {
                    AgreedPowerPricePerBlock[i] = 0;
                }
                else
                {
                    AgreedPowerPricePerBlock[i] = calculationOptions.AgreedMaxPowerBlocks[i] * BlockPrices.GetCombinedPowerPricePerKW(calculationOptions, i);
                }
            }
            AgreedPowerPrice = AgreedPowerPricePerBlock.Sum();
            OldFixedPrice = calculationOptions.ObracunskaMoc * BlockPrices.GetFixedPricePerKW(calculationOptions);

            var overdraftsPerBlock = new decimal[5];
            var overdraftsPerBlockPowerOf2 = new decimal[5];
            var usageCountPerBlock = new int[5];
            foreach (var usage in rawUsages)
            {
                var maxUsage = calculationOptions.AgreedMaxPowerBlocks[usage.Block];
                var overDraftPower = usage.ImportPower - maxUsage;
                if (overDraftPower > 0)
                {
                    overdraftsPerBlock[usage.Block] += overDraftPower;
                    overdraftsPerBlockPowerOf2[usage.Block] += overDraftPower * overDraftPower;
                }
                usageCountPerBlock[usage.Block]++;
            }

            for (int i = 0; i < 5; i++)
            {
                var overdraftInBlockSquaredAndFactored =
                    (decimal)Math.Sqrt((double)overdraftsPerBlockPowerOf2[i]) *
                    BlockPrices.GetFactor(calculationOptions.SimulationYear);

                OverdraftPowerPricePerBlock[i] =
                    overdraftInBlockSquaredAndFactored * BlockPrices.GetCombinedPowerPricePerKW(calculationOptions, i);
                OverdraftPowerPrice += OverdraftPowerPricePerBlock[i];
            }

            if (rawUsages.Count == 0)
            {
                if (manualMonthEnergy == null)
                {
                    throw new InvalidOperationException("No data");
                }
                if (!calculationOptions.Has15MinuteData)
                {
                    FixedPowerPriceIfNo15Minute = BlockPrices.GetCombinedPowerPriceNo15Minutes(calculationOptions);
                }
                else
                {
                    FixedPowerPriceIfNo15Minute = 0;
                }
                if (calculationOptions.TwoTariffSystem)
                {
                    EnergyTransferHighTariffPrice = manualMonthEnergy.HighTarif * BlockPrices.GetNonBlockTransferEnergyPerKWH(true, calculationOptions);
                    EnergyTransferLowTariffPrice = manualMonthEnergy.LowTarif * BlockPrices.GetNonBlockTransferEnergyPerKWH(false, calculationOptions);
                    OldEnergyTransferHighTariffPrice = manualMonthEnergy.HighTarif * BlockPrices.GetOldTransferEnergyPricePerKWH(true, calculationOptions);
                    OldEnergyTransferLowTariffPrice = manualMonthEnergy.LowTarif * BlockPrices.GetOldTransferEnergyPricePerKWH(false, calculationOptions);
                    EnergyTransferPrice = EnergyTransferHighTariffPrice + EnergyTransferLowTariffPrice;
                    OldEnergyTransferPrice = OldEnergyTransferHighTariffPrice + OldEnergyTransferLowTariffPrice;
                }
                else
                {
                    EnergyTransferHighTariffPrice = 0;
                    EnergyTransferLowTariffPrice = 0;
                    OldEnergyTransferHighTariffPrice = 0;
                    OldEnergyTransferLowTariffPrice = 0;

                    EnergyTransferPrice = manualMonthEnergy.SingleTarif * BlockPrices.GetNonBlockTransferEnergyPerKWH(null, calculationOptions);
                    OldEnergyTransferPrice = manualMonthEnergy.SingleTarif * BlockPrices.GetOldTransferEnergyPricePerKWH(null, calculationOptions);
                }
                return;
            }
            else
            {
                FixedPowerPriceIfNo15Minute = 0;
            }

            foreach (var usage in rawUsages)
            {
                var day = DateOnly.FromDateTime(usage.DateTime);
                if (!DailyReports.TryGetValue(day, out var dayReport))
                {
                    DailyReports[day] = dayReport = new DayReport { Day = day };
                }
                var maxUsage = calculationOptions.AgreedMaxPowerBlocks[usage.Block];
                var overDraftPower = Math.Max(0, usage.ImportPower - maxUsage);
                var overdraftPrice = overdraftsPerBlock[usage.Block] == 0 ? 0 : OverdraftPowerPricePerBlock[usage.Block] * (overDraftPower / overdraftsPerBlock[usage.Block]);

                dayReport.Usages.Add(
                    new(
                        usage,
                        overDraftPower,
                        overdraftPrice,
                        AgreedPowerPricePerBlock[usage.Block] / usageCountPerBlock[usage.Block]
                    ));
            }

            var dailyOldPrice = DailyReports.Count == 0 ? 0 : OldFixedPrice / DailyReports.Count;
            foreach (var dailyUsage in DailyReports.OrderBy(d => d.Key))
            {
                foreach (var usage in dailyUsage.Value.Usages)
                {
                    dailyUsage.Value.OverdraftPowerPricePerBlock[usage.Source.Block] += usage.OverdraftPrice;
                    dailyUsage.Value.OverdraftPowerPrice += usage.OverdraftPrice;
                    dailyUsage.Value.AgreedPowerPrice += usage.AgreedPowerPrice;
                    dailyUsage.Value.AgreedPowerPricePerBlock[usage.Source.Block] += usage.AgreedPowerPrice;

                    dailyUsage.Value.OldFixedPrice = dailyOldPrice;

                    EnergyImportPerBlockInKWh[usage.Source.Block] += usage.Source.ImportEnergy;
                    if (usage.Source.HighTariff)
                    {
                        dailyUsage.Value.HighTariffImportEnergyInKWh += usage.Source.ImportEnergy;
                        HighTariffImportEnergyInKWh += usage.Source.ImportEnergy;
                        dailyUsage.Value.HighTariffExportEnergyInKWh += usage.Source.ExportEnergy;
                        HighTariffExportEnergyInKWh += usage.Source.ExportEnergy;
                    }
                    else
                    {
                        dailyUsage.Value.LowTariffImportEnergyInKWh += usage.Source.ImportEnergy;
                        LowTariffImportEnergyInKWh += usage.Source.ImportEnergy;
                        dailyUsage.Value.LowTariffExportEnergyInKWh += usage.Source.ExportEnergy;
                        LowTariffExportEnergyInKWh += usage.Source.ExportEnergy;
                    }
                }

                if (calculationOptions.SolarPowerPlant == "Imam" && calculationOptions.NetMetering)
                {
                    dailyUsage.Value.EnergyTransferPrice = 0;
                    EnergyTransferPrice = 0;
                    dailyUsage.Value.OldEnergyTransferPrice = 0;
                    OldEnergyTransferPrice = 0;
                }
                else
                {
                    if (calculationOptions.TwoTariffSystem)
                    {
                        dailyUsage.Value.EnergyTransferPrice = dailyUsage.Value.HighTariffImportEnergyInKWh * BlockPrices.GetNonBlockTransferEnergyPerKWH(true, calculationOptions)
                            + dailyUsage.Value.LowTariffImportEnergyInKWh * BlockPrices.GetNonBlockTransferEnergyPerKWH(false, calculationOptions);
                        dailyUsage.Value.OldEnergyTransferPrice = dailyUsage.Value.HighTariffImportEnergyInKWh * BlockPrices.GetOldTransferEnergyPricePerKWH(true, calculationOptions)
                            + dailyUsage.Value.LowTariffImportEnergyInKWh * BlockPrices.GetOldTransferEnergyPricePerKWH(false, calculationOptions);
                        if (calculationOptions.IncludeEnergyPrice)
                        {
                            dailyUsage.Value.EnergyPrice = (dailyUsage.Value.HighTariffImportEnergyInKWh * calculationOptions.HighTariffEnergyPrice
                                + dailyUsage.Value.LowTariffImportEnergyInKWh * calculationOptions.LowTariffEnergyPrice) * calculationOptions.VATMultiplier;
                        }
                        else
                        {
                            dailyUsage.Value.EnergyPrice = 0;
                        }
                    }
                    else
                    {
                        var singleTariffImportEnergyInKWh = dailyUsage.Value.HighTariffImportEnergyInKWh + dailyUsage.Value.LowTariffImportEnergyInKWh;
                        dailyUsage.Value.EnergyTransferPrice = singleTariffImportEnergyInKWh * BlockPrices.GetNonBlockTransferEnergyPerKWH(null, calculationOptions);
                        dailyUsage.Value.OldEnergyTransferPrice = singleTariffImportEnergyInKWh * BlockPrices.GetOldTransferEnergyPricePerKWH(null, calculationOptions);
                        if (calculationOptions.IncludeEnergyPrice)
                        {
                            dailyUsage.Value.EnergyPrice = singleTariffImportEnergyInKWh * calculationOptions.SingleTariffEnergyPrice;
                        }
                        else
                        {
                            dailyUsage.Value.EnergyPrice = 0;
                        }
                    }
                    EnergyTransferPrice += dailyUsage.Value.EnergyTransferPrice;
                    OldEnergyTransferPrice += dailyUsage.Value.OldEnergyTransferPrice;
                    EnergyPrice += dailyUsage.Value.EnergyPrice;
                }
            }
        }
    }
}
