using System.Diagnostics;
using System.Globalization;

namespace Omreznina.Client.Logic
{
    public class CalculationOptions
    {
        public decimal[] AgreedMaxPowerBlocks { get; set; }
        public int SimulationYear { get; set; }
        public bool IncludeVAT { get; set; }
        public string OldPricelist { get; set; } = "2023.11.1";
        public decimal VarovalkePower { get; set; }
        public bool TwoTariffSystem { get; set; } = true;

        public CalculationOptions(decimal[] AgreedMaxPower, int SimulationYear, bool IncludeVAT, decimal VarovalkePower)
        {
            AgreedMaxPowerBlocks = AgreedMaxPower;
            this.SimulationYear = SimulationYear;
            this.IncludeVAT = IncludeVAT;
            this.VarovalkePower = VarovalkePower;
        }
    }

    public record RawUsage15Min(DateTime DateTime, int Block, bool HighTariff, decimal Power, decimal Energy);
    public record CalculatedUsage(RawUsage15Min Source, decimal OverdraftPower, decimal EnergyTansportPrice, decimal OverdraftPrice, decimal AgreedPowerPrice, decimal OldEnergyPrice);

    public class OmrezninaReport : PeriodReport
    {
        public MonthlyReport[] MonthlyReports { get; }
        public CalculationOptions CalculationOptions { get; }

        public OmrezninaReport(CalculationOptions calculationOptions, Dictionary<(int Year, int Month), List<RawUsage15Min>> rawUsages)
        {
            CalculationOptions = calculationOptions;
            MonthlyReports = rawUsages.Select(m => new MonthlyReport(calculationOptions, m.Key, m.Value)).OrderBy(m => m.Month).ToArray();
            EnergyPrice = MonthlyReports.Sum(m => m.EnergyPrice);
            OldEnergyPrice = MonthlyReports.Sum(m => m.OldEnergyPrice);
            OldFixedPrice = MonthlyReports.Sum(m => m.OldFixedPrice);
            AgreedPowerPrice = MonthlyReports.Sum(m => m.AgreedPowerPrice);
            OverdraftPowerPrice = MonthlyReports.Sum(m => m.OverdraftPowerPrice);
            for (int i = 0; i < 5; i++)
            {
                OverdraftPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.OverdraftPowerPricePerBlock[i]);
                AgreedPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.AgreedPowerPricePerBlock[i]);
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
                var maxPowerColumn = csv.GetOrdinal("P+ Prejeta delovna moč");
                var consumedEnergyColumn = csv.GetOrdinal("Energija A+");
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
                    if (csv.GetFieldSpan(maxPowerColumn).IsEmpty)
                        continue;
                    var power = decimal.Parse(csv.GetFieldSpan(maxPowerColumn), CultureInfo.InvariantCulture);
                    var energy = csv.GetDecimal(consumedEnergyColumn);
                    var myBlock = dateTime.ToBlock();
                    var theirBlock = csv.GetInt32(blockColumn);
                    if (myBlock + 1 != theirBlock)
                        throw new InvalidOperationException();
                    parsedUsage.Add(new(dateTime, myBlock, dateTime.IsHighTariff(), power, energy));
                }
            }
            return parsedUsageAll;
        }
    }

    public class PeriodReport
    {
        public decimal OldFixedPrice { get; set; }
        public decimal OldEnergyPrice { get; set; }
        public decimal EnergyPrice { get; set; }
        public decimal AgreedPowerPrice { get; set; }
        public decimal OverdraftPowerPrice { get; set; }
        public decimal[] OverdraftPowerPricePerBlock { get; } = new decimal[5];
        public decimal[] AgreedPowerPricePerBlock { get; } = new decimal[5];
    }

    public class DayReport : PeriodReport
    {
        public DateOnly Day { get; init; }
        public List<CalculatedUsage> Usages { get; } = new();
        public int Index { get; internal set; }
    }

    public class MonthlyReport : PeriodReport
    {
        public (int Year, int Month) Month { get; }
        public Dictionary<DateOnly, DayReport> DailyReports { get; } = new();

        public MonthlyReport(CalculationOptions calculationOptions, (int Year, int Month) month, List<RawUsage15Min> rawUsages)
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
                AgreedPowerPricePerBlock[i] = calculationOptions.AgreedMaxPowerBlocks[i] * BlockPrices.GetDistributionPowerPricePerKW(calculationOptions.IncludeVAT, i)
                + calculationOptions.AgreedMaxPowerBlocks[i] * BlockPrices.GetTransferPowerPricePerKW(calculationOptions.IncludeVAT, i);
            }
            AgreedPowerPrice = AgreedPowerPricePerBlock.Sum();
            OldFixedPrice = calculationOptions.VarovalkePower * BlockPrices.GetFixedPricePerKW(calculationOptions.IncludeVAT, calculationOptions.OldPricelist);

            var overdraftsPerBlock = new decimal[5];
            var overdraftsPerBlockPowerOf2 = new decimal[5];
            var usageCountPerBlock = new int[5];
            foreach (var usage in rawUsages)
            {
                var maxUsage = calculationOptions.AgreedMaxPowerBlocks[usage.Block];
                var overDraftPower = usage.Power - maxUsage;
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
                    overdraftInBlockSquaredAndFactored * BlockPrices.GetDistributionPowerPricePerKW(calculationOptions.IncludeVAT, i) +
                    overdraftInBlockSquaredAndFactored * BlockPrices.GetTransferPowerPricePerKW(calculationOptions.IncludeVAT, i);
                OverdraftPowerPrice += OverdraftPowerPricePerBlock[i];
            }

            foreach (var usage in rawUsages)
            {
                var day = DateOnly.FromDateTime(usage.DateTime);
                if (!DailyReports.TryGetValue(day, out var dayReport))
                {
                    DailyReports[day] = dayReport = new DayReport { Day = day };
                }
                var maxUsage = calculationOptions.AgreedMaxPowerBlocks[usage.Block];
                var overDraftPower = Math.Max(0, usage.Power - maxUsage);
                var energyTransportPrice = usage.Energy * BlockPrices.GetDistributionEnergyPricePerKWH(calculationOptions.IncludeVAT, usage.Block) +
                    usage.Energy * BlockPrices.GetTransferEnergyPricePerKWH(calculationOptions.IncludeVAT, usage.Block);
                var overdraftPrice = overdraftsPerBlock[usage.Block] == 0 ? 0 : OverdraftPowerPricePerBlock[usage.Block] * (overDraftPower / overdraftsPerBlock[usage.Block]);
                dayReport.Usages.Add(
                    new(
                        usage,
                        overDraftPower,
                        energyTransportPrice,
                        overdraftPrice,
                        AgreedPowerPricePerBlock[usage.Block] / usageCountPerBlock[usage.Block],
                        usage.Energy * BlockPrices.GetOldTransferEnergyPricePerKWH(calculationOptions.IncludeVAT,
                        calculationOptions.TwoTariffSystem ? usage.HighTariff : null,
                        calculationOptions.OldPricelist)
                        )
                    );
            }

            var dailyOldPrice = OldFixedPrice / DailyReports.Count;
            int dayIndex = 0;
            foreach (var dailyUsage in DailyReports.OrderBy(d => d.Key))
            {
                dailyUsage.Value.Index = dayIndex++;
                foreach (var usage in dailyUsage.Value.Usages)
                {
                    dailyUsage.Value.OverdraftPowerPricePerBlock[usage.Source.Block] += usage.OverdraftPrice;
                    dailyUsage.Value.OverdraftPowerPrice += usage.OverdraftPrice;
                    dailyUsage.Value.AgreedPowerPrice += usage.AgreedPowerPrice;
                    dailyUsage.Value.AgreedPowerPricePerBlock[usage.Source.Block] += usage.AgreedPowerPrice;

                    dailyUsage.Value.OldFixedPrice = dailyOldPrice;

                    dailyUsage.Value.EnergyPrice += usage.EnergyTansportPrice;
                    EnergyPrice += usage.EnergyTansportPrice;

                    dailyUsage.Value.OldEnergyPrice += usage.OldEnergyPrice;
                    OldEnergyPrice += usage.OldEnergyPrice;
                }
            }
        }
    }
}
