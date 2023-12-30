using Omreznina.Client.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Omreznina.Client.Logic
{
    public class CalculationOptions
    {
        public static (string Text, int ObracunskaMoc, int PrikljucnaMoc, bool ThreePhase)[] AllBreakersOptions =
             [
            ("3kW (1x16 A)", 3, 4, false),
            ("3kW (1x20 A)", 3, 5, false),
            ("6kW (1x25 A)", 6, 6, false),
            ("7kW (1x32 A)", 7, 7, false),
            ("7kW (1x35 A)", 7, 8, false),

            ("7kW (3x16 A)", 7, 11  , true),
            ("7kW (3x20 A)", 7, 14  , true),
            ("10kW (3x25 A)", 10, 17, true),
            ("22kW (3x32 A)", 22, 22, true),
            ("24kW (3x35 A)", 24, 24, true),
            ("28kW (3x40 A)", 28, 28, true),
            ("35kW (3x50 A)", 35, 35, true),
            ("43kW (3x63 A)", 43, 43, true)
            ];

        private Dictionary<string, (int ObracunskaMoc, int PrikjucnaMoc, bool ThreePhase)> MappedPowerBreakers;

        public class AgreedPowerBlocks : ObservableCollection<decimal>
        {
            public event Action<string>? ErrorMessage;

            protected override void SetItem(int index, decimal newValue)
            {
                if (index > 4)
                    throw new System.Exception("Max 5 blocks");
                if (index < 0)
                    throw new IndexOutOfRangeException();

                if (newValue < minimumPowerForBlocks)
                {
                    newValue = minimumPowerForBlocks;
                    ErrorMessage?.Invoke($"Moč prvega bloka mora biti večja ali enaka {(minimumPowerForBlocks.ToKW())} glede na moč varovalk na podlagi 4. odstavka 12. člena \"Akta o metodologiji za obračunavanje omrežnine za elektrooperaterje\"");
                }
                if (newValue > maximumPowerForBlocks)
                {
                    newValue = maximumPowerForBlocks;
                    ErrorMessage?.Invoke($"Moč blokov mora biti manjša ali enaka moči varovalk");
                }
                base.SetItem(index, newValue);
                if (index + 1 < Count && this[index + 1] < newValue)
                {
                    this[index + 1] = newValue;
                    ErrorMessage?.Invoke($"Moč {index + 2}. bloka se je prilagodila moči {index + 1}. bloka, saj ne sme biti manjša na podlagi 10. odstavka 12. člena \"Akta o metodologiji za obračunavanje omrežnine za elektrooperaterje\"");
                }
                if (index > 0 && this[index - 1] > newValue)
                {
                    this[index - 1] = newValue;
                    ErrorMessage?.Invoke($"Moč {index}. bloka se je prilagodila moči {index + 1}. bloka, saj ne sme biti večja na podlagi 10. odstavka 12. člena \"Akta o metodologiji za obračunavanje omrežnine za elektrooperaterje\"");
                }
            }

            private decimal minimumPowerForBlocks;
            private decimal maximumPowerForBlocks;

            /// <summary>
            /// (4) Minimalna dogovorjena obračunska moč za časovni blok 1 uporabnika sistema se določi:
            ///    - za enofazni priključek uporabnika sistema s priključno močjo enako ali manjšo od 43 kW, kot 31 % priključne moči iz soglasja za priključitev, vendar ne manj kot 2,0 kW;
            ///    - za trifazne priključke s priključno močjo enako ali manjšo od 43 kW, kot 27 % priključne moči iz soglasja za priključitev, vendar ne manj kot 3,5 kW za uporabnike sistema s priključno močjo do vključno 17 kW;
            ///    - za trifazne priključke s priključno močjo enako ali manjšo od 43 kW ter za uporabnike sistema iz drugega odstavka 37. člena tega akta, kot 34 % priključne moči iz soglasja za priključitev, za uporabnike sistema s priključno močjo nad 17 kW;
            ///    - za uporabnike sistema s priključno močjo nad 43 kW kot 25 % priključne moči.
            /// </summary>
            public void SetVarovalkePower((int ObracunskaMoc, int PrikljucnaMoc, bool ThreePhase) breakers)
            {
                var prikljucnaMoc = (decimal)breakers.PrikljucnaMoc;
                var oldMinimalPowerForBlock1 = minimumPowerForBlocks;
                if (breakers.ThreePhase)
                {
                    if (prikljucnaMoc <= 17)
                        minimumPowerForBlocks = Math.Max(3.5M, 0.27M * prikljucnaMoc);
                    else if (prikljucnaMoc <= 43)
                        minimumPowerForBlocks = 0.34M * prikljucnaMoc;
                    else
                        minimumPowerForBlocks = 0.25M * breakers.ObracunskaMoc;
                }
                else
                {
                    if (prikljucnaMoc <= 43)
                        minimumPowerForBlocks = Math.Max(2M, 0.31M * prikljucnaMoc);
                    else
                        minimumPowerForBlocks = 0.25M * breakers.ObracunskaMoc;
                }
                minimumPowerForBlocks = Math.Round(minimumPowerForBlocks, 1);
                if (oldMinimalPowerForBlock1 != minimumPowerForBlocks)
                {
                    if (this[0] < minimumPowerForBlocks)
                        this[0] = minimumPowerForBlocks;
                }
                var oldMaximumPowerForBlocks = maximumPowerForBlocks;
                maximumPowerForBlocks = Math.Round(prikljucnaMoc, 1);
                if (maximumPowerForBlocks != oldMaximumPowerForBlocks)
                {
                    if (this[4] > maximumPowerForBlocks)
                        this[4] = maximumPowerForBlocks;
                }
            }
        }

        public AgreedPowerBlocks AgreedMaxPowerBlocks { get; set; } = [7.5M, 8, 9, 10, 11];
        public int SimulationYear { get; set; } = 2024;
        public bool IncludeVAT { get; set; } = false;
        public bool NetMetering { get; set; } = false;
        public string OldPricelist { get; set; } = "2023.11.1";

        private string breakersText = "10kW (3x25 A)";
        public string BreakersText
        {
            get
            {
                return breakersText;
            }
            set
            {
                breakersText = value;
                AgreedMaxPowerBlocks.SetVarovalkePower(BreakersValue);
            }
        }

        public (int ObracunskaMoc, int PrikljucnaMoc, bool ThreePhase) BreakersValue
        {
            get => MappedPowerBreakers[breakersText];
        }

        public bool TwoTariffSystem { get; set; } = true;

        public CalculationOptions()
        {
            MappedPowerBreakers = AllBreakersOptions.ToDictionary(v => v.Text, v => (v.ObracunskaMoc, v.PrikljucnaMoc, v.ThreePhase));
            AgreedMaxPowerBlocks.SetVarovalkePower(BreakersValue);
        }
    }

    public record RawUsage15Min(DateTime DateTime, int Block, bool HighTariff, decimal ConsumedPower, decimal ConsumedEnergy, decimal GivenPower, decimal GivenEnergy);
    public record CalculatedUsage(RawUsage15Min Source, decimal OverdraftPower, decimal EnergyTansportPrice, decimal OverdraftPrice, decimal AgreedPowerPrice, decimal OldEnergyPrice);

    public class OmrezninaReport : PeriodReport
    {
        public MonthlyReport[] MonthlyReports { get; }
        public CalculationOptions CalculationOptions { get; }

        public OmrezninaReport(CalculationOptions calculationOptions, Dictionary<(int Year, int Month), List<RawUsage15Min>> rawUsages)
        {
            CalculationOptions = calculationOptions;
            if (rawUsages == null)
            {
                MonthlyReports=Enumerable.Range(1, 12).Select(m => new MonthlyReport(calculationOptions, (2021, m), new List<RawUsage15Min>())).ToArray();
            }
            else
            {
                MonthlyReports = rawUsages.Select(m => new MonthlyReport(calculationOptions, m.Key, m.Value)).OrderBy(m => m.Month).ToArray();
            }
            NetMeteringEnergyInKWh = Math.Max(0M, MonthlyReports.Sum(m => m.NetMeteringEnergyInKWh));
            if (calculationOptions.NetMetering)
            {
                EnergyPrice = NetMeteringEnergyInKWh * BlockPrices.GetCombinedEnergyPriceSingleTariffPerKWH(calculationOptions.IncludeVAT);
                OldEnergyPrice = NetMeteringEnergyInKWh * BlockPrices.GetOldEnergyPriceSingleTariffPerKWh(calculationOptions);
            }
            else
            {
                EnergyPrice = MonthlyReports.Sum(m => m.EnergyPrice);
                OldEnergyPrice = MonthlyReports.Sum(m => m.OldEnergyPrice);
            }
            OldFixedPrice = MonthlyReports.Sum(m => m.OldFixedPrice);
            AgreedPowerPrice = MonthlyReports.Sum(m => m.AgreedPowerPrice);
            OverdraftPowerPrice = MonthlyReports.Sum(m => m.OverdraftPowerPrice);
            for (int i = 0; i < 5; i++)
            {
                OverdraftPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.OverdraftPowerPricePerBlock[i]);
                AgreedPowerPricePerBlock[i] = MonthlyReports.Sum(m => m.AgreedPowerPricePerBlock[i]);
                EnergyPerBlockInKWh[i] = MonthlyReports.Sum(m => m.EnergyPerBlockInKWh[i]);
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
                var takenPowerColumn = csv.GetOrdinal("P+ Prejeta delovna moč");
                var givenPowerColumn = csv.GetOrdinal("P- Oddana delovna moč");
                var consumedEnergyColumn = csv.GetOrdinal("Energija A+");
                var givenEnergyColumn = csv.GetOrdinal("Energija A-");
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
                    if (csv.GetFieldSpan(takenPowerColumn).IsEmpty)
                        continue;
                    var consumedPower = decimal.Parse(csv.GetFieldSpan(takenPowerColumn), CultureInfo.InvariantCulture);
                    var givenPower = csv.GetFieldSpan(givenPowerColumn).IsEmpty ? 0M : decimal.Parse(csv.GetFieldSpan(givenPowerColumn), CultureInfo.InvariantCulture);
                    var consumedEnergy = csv.GetDecimal(consumedEnergyColumn);
                    var givenEnergy = csv.GetFieldSpan(givenEnergyColumn).IsEmpty ? 0M : decimal.Parse(csv.GetFieldSpan(givenEnergyColumn), CultureInfo.InvariantCulture);
                    var theirBlock = csv.GetInt32(blockColumn) - 1;
                    //var myBlock = dateTime.ToBlock();
                    //if (myBlock != theirBlock)
                    //    Console.WriteLine($"My block:{myBlock} Their block:{theirBlock}");
                    parsedUsage.Add(new(dateTime, theirBlock, dateTime.IsHighTariff(), consumedPower, consumedEnergy, givenPower, givenEnergy));
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
        public decimal NetMeteringEnergyInKWh { get; set; }
        public decimal AgreedPowerPrice { get; set; }
        public decimal OverdraftPowerPrice { get; set; }
        public decimal[] OverdraftPowerPricePerBlock { get; } = new decimal[5];
        public decimal[] AgreedPowerPricePerBlock { get; } = new decimal[5];
        public decimal[] EnergyPerBlockInKWh { get; } = new decimal[5];
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
            OldFixedPrice = calculationOptions.BreakersValue.ObracunskaMoc * BlockPrices.GetFixedPricePerKW(calculationOptions.IncludeVAT, calculationOptions.OldPricelist);

            var overdraftsPerBlock = new decimal[5];
            var overdraftsPerBlockPowerOf2 = new decimal[5];
            var usageCountPerBlock = new int[5];
            foreach (var usage in rawUsages)
            {
                var maxUsage = calculationOptions.AgreedMaxPowerBlocks[usage.Block];
                var overDraftPower = usage.ConsumedPower - maxUsage;
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
                    overdraftInBlockSquaredAndFactored * BlockPrices.GetCombinedPowerPricePerKW(calculationOptions.IncludeVAT, i);
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
                var overDraftPower = Math.Max(0, usage.ConsumedPower - maxUsage);
                var overdraftPrice = overdraftsPerBlock[usage.Block] == 0 ? 0 : OverdraftPowerPricePerBlock[usage.Block] * (overDraftPower / overdraftsPerBlock[usage.Block]);

                var energyTransportPrice = calculationOptions.NetMetering ? 0 : usage.ConsumedEnergy * BlockPrices.GetCombinedEnergyPricePerKWH(calculationOptions.IncludeVAT, usage.Block);
                var oldEnergyPrice = calculationOptions.NetMetering ? 0 : usage.ConsumedEnergy * BlockPrices.GetOldTransferEnergyPricePerKWH(calculationOptions.IncludeVAT,
                                                                                                           calculationOptions.TwoTariffSystem ? usage.HighTariff : null,
                                                                                                           calculationOptions.OldPricelist);

                dayReport.Usages.Add(
                    new(
                        usage,
                        overDraftPower,
                        energyTransportPrice,
                        overdraftPrice,
                        AgreedPowerPricePerBlock[usage.Block] / usageCountPerBlock[usage.Block],
                        oldEnergyPrice
                        )
                    );
            }

            var dailyOldPrice = DailyReports.Count == 0 ? 0 : OldFixedPrice / DailyReports.Count;
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
                    EnergyPerBlockInKWh[usage.Source.Block] += usage.Source.ConsumedEnergy;

                    //dailyUsage.Value.NetMeteringEnergyInKWh += usage.Source.ConsumedEnergy;
                    NetMeteringEnergyInKWh += usage.Source.ConsumedEnergy;
                    //dailyUsage.Value.NetMeteringEnergyInKWh -= usage.Source.GivenEnergy;
                    NetMeteringEnergyInKWh -= usage.Source.GivenEnergy;

                    dailyUsage.Value.OldEnergyPrice += usage.OldEnergyPrice;
                    OldEnergyPrice += usage.OldEnergyPrice;
                }
            }
        }
    }
}
