using CommunityToolkit.Mvvm.ComponentModel;
using Omreznina.Client.Pages;
using System.Collections.ObjectModel;

namespace Omreznina.Client.Logic
{
    public partial class CalculationOptions : ObservableObject
    {
        public static (string Text, int ObracunskaMocGospodinjstva, int ObracunskaMocMalaPodjetja, int PrikljucnaMoc, bool ThreePhase, int Amps)[] AllBreakersOptions =
             [
            ("3kW (1x16 A)", 3, 3, 4, false, 16),
            ("3kW (1x20 A)", 3, 5, 5, false, 20),
            ("6kW (1x25 A)", 6, 6, 6, false, 25),
            ("7kW (1x32 A)", 7, 7, 7, false, 32),
            ("7kW (1x35 A)", 7, 8, 8, false, 35),

            ("7kW (3x16 A)", 7, 11, 11  , true, 16),
            ("7kW (3x20 A)", 7, 14, 14  , true, 20),
            ("10kW (3x25 A)", 10, 17, 17, true, 25),
            ("22kW (3x32 A)", 22, 22, 22, true, 32),
            ("24kW (3x35 A)", 24, 24, 24, true, 35),
            ("28kW (3x40 A)", 28, 28, 28, true, 40),
            ("35kW (3x50 A)", 35, 35, 35, true, 50),
            ("43kW (3x63 A)", 43, 43, 43, true, 63)
            ];

        public string MeterType
        {
            get => meterType;
            set
            {
                meterType = value;
                if (AllMeterTypes[0] == value)
                {
                    ConnectionType = "Nizka napetost(NN), uporabniška skupina 0";
                    VrstaOdjema = "Gospodinjstvo";
                    ObracunskaMoc = BreakersValue.ObracunskaMocGospodinjstva;

                }
                else if (AllMeterTypes[1] == value)
                {
                    ConnectionType = "Nizka napetost(NN), uporabniška skupina 0";
                    VrstaOdjema = "Brez merjenja moči";
                    ObracunskaMoc = BreakersValue.ObracunskaMocGospodinjstva;
                }
                else
                {
                    TwoTariffSystem = true;
                }
            }
        }
        public static string[] AllMeterTypes = ["Gospodinjstvo", "Mali poslovni odjemalec(do 43kW)", "Drugo"];
        public string ConnectionType
        {
            get => connectionType; set
            {
                if (connectionType != value)
                {
                    connectionType = value;
                    VrstaOdjema = OldPriceList[value].Keys.First();
                }
                connectionType = value;
                UserGroup = int.Parse(value.Substring(value.Length - 1));
            }
        }
        public int UserGroup { get; private set; } = 0;
        public string VrstaOdjema { get; set; } = "Gospodinjstvo";

        public static Dictionary<string, Dictionary<string, (decimal OM, decimal VT, decimal MT, decimal ET)>> OldPriceList = new() {
            {"Nizka napetost(NN), uporabniška skupina 0",
                new(){
                    {"T ≥ 2500 ur", (5.71190M, 0.01689M, 0.01298M, -1M) },
                    {"T < 2500 ur", (4.74796M, 0.02290M, 0.01759M, -1M) },
                    {"Polnjenje EV", (2.37398M, 0.01144M, 0.00882M, -1M) },
                    {"Brez merjenja moči", (0.79600M, 0.04308M, 0.03311M, 0.03973M) },
                    {"Gospodinjstvo", (0.79600M, 0.04308M, 0.03311M, 0.03973M) }
                }
            },
            {"NN na zbiralnici NN v TP, uporabniška skupina 1",
                new() {
                    {"T ≥ 2500 ur", (4.33074M, 0.00765M, 0.00592M, -1M) },
                    {"T < 2500 ur", (3.60756M, 0.01218M, 0.00936M, -1M) }
                }
            },
            {"Srednja napetost(SN), uporabniška skupina 2",
                new() {
                    {"T ≥ 2500 ur", (3.22148M, 0.00789M, 0.00608M, -1M) },
                    {"T < 2500 ur", (2.47536M, 0.01252M, 0.00964M, -1M) }
                }
            },
            {"SN na zbiralnici SN v RTP, uporabniška skupina 3",
                new() {
                    {"T ≥ 2500 ur", (3.09043M, 0.00074M, 0.00057M, -1M) },
                    {"T < 2500 ur", (3.05375M, 0.00097M, 0.00075M, -1M) }
                }
            },
            {"Visoka napetost(VN), uporabniška skupina 4",
                new(){
                    {"T ≥ 6000 ur", (0.95460M, 0.00158M, 0.00123M, -1M) },
                    {"6000 > T ≥ 2500 ur", (1.02089M, 0.00145M, 0.00111M, -1M) },
                    {"T < 2500 ur", (1.10050M, 0.00153M, 0.00118M, -1M) }
                }
            },
        };

        private Dictionary<string, (int ObracunskaMocGospodinjstva, int ObracunskaMocMalaPodjetja, int PrikjucnaMoc, bool ThreePhase)> MappedPowerBreakers;

        public class AgreedPowerBlocks : ObservableCollection<decimal>
        {
            public event Action<string>? ErrorMessage;

            public void ResetToMinimal()
            {
                for (int i = 0; i < 5; i++)
                {
                    this[i] = minimumPowerForBlocks;
                }
            }

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
            public void SetVarovalkePower(int PrikljucnaMoc, bool ThreePhase, int ObracunskaMoc)
            {
                var prikljucnaMoc = (decimal)PrikljucnaMoc;
                var oldMinimalPowerForBlock1 = minimumPowerForBlocks;
                if (ThreePhase)
                {
                    if (prikljucnaMoc <= 17)
                        minimumPowerForBlocks = Math.Max(3.5M, 0.27M * prikljucnaMoc);
                    else if (prikljucnaMoc <= 43)
                        minimumPowerForBlocks = 0.34M * prikljucnaMoc;
                    else
                        minimumPowerForBlocks = 0.25M * ObracunskaMoc;
                }
                else
                {
                    if (prikljucnaMoc <= 43)
                        minimumPowerForBlocks = Math.Max(2M, 0.31M * prikljucnaMoc);
                    else
                        minimumPowerForBlocks = 0.25M * ObracunskaMoc;
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
        public decimal VATMultiplier { get => IncludeVAT ? 1.22M : 1M; }
        public bool NetMetering
        {
            get => netMetering; set
            {
                netMetering = value;
                if (value)
                {
                    TwoTariffSystem = false;
                }
            }
        }

        private string? breakersText = null;
        private bool netMetering = false;
        private string meterType = "Gospodinjstvo";
        private int obracunskaMoc;
        private string connectionType = "Nizka napetost(NN), uporabniška skupina 0";
        [ObservableProperty]
        private bool has15MinuteData = true;

        public string? BreakersText
        {
            get
            {
                return breakersText;
            }
            set
            {
                breakersText = value;
                if (meterType == "Gospodinjstvo")
                {
                    ObracunskaMoc = BreakersValue.ObracunskaMocGospodinjstva;
                }
                else
                {
                    ObracunskaMoc = BreakersValue.ObracunskaMocMalaPodjetja;
                }
            }
        }
        public int ObracunskaMoc
        {
            get => obracunskaMoc; set
            {
                obracunskaMoc = value;
                if (MeterType == "Drugo")
                {
                    AgreedMaxPowerBlocks.SetVarovalkePower(999, true, ObracunskaMoc);
                }
                else
                {
                    AgreedMaxPowerBlocks.SetVarovalkePower(BreakersValue.PrikljucnaMoc, BreakersValue.ThreePhase, ObracunskaMoc);
                }
            }
        }
        public (int ObracunskaMocGospodinjstva, int ObracunskaMocMalaPodjetja, int PrikljucnaMoc, bool ThreePhase) BreakersValue
        {
            get => breakersText == null ? (0, 0, 0, false) : MappedPowerBreakers.TryGetValue(breakersText, out var val) ? val : (0, 0, 0, false);
        }

        public bool TwoTariffSystem { get; set; } = true;
        [ObservableProperty]
        public bool includeEnergyPrice = false;
        [ObservableProperty]
        public decimal highTariffEnergyPrice = 0.11800M;
        [ObservableProperty]
        public decimal lowTariffEnergyPrice = 0.08200M;
        [ObservableProperty]
        public decimal singleTariffEnergyPrice = 0.09800M;

        public CalculationOptions()
        {
            MappedPowerBreakers = AllBreakersOptions.ToDictionary(v => v.Text, v => (v.ObracunskaMocGospodinjstva, v.ObracunskaMocMalaPodjetja, v.PrikljucnaMoc, v.ThreePhase));
            AgreedMaxPowerBlocks.SetVarovalkePower(BreakersValue.PrikljucnaMoc, BreakersValue.ThreePhase, ObracunskaMoc);
        }
    }
}
