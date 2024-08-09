using CommunityToolkit.Mvvm.ComponentModel;
using Omreznina.Client.Logic;
using Omreznina.Models;
using Omreznina.Pages;
using Omreznina.Pages.SettingsPages;

namespace Omreznina.Logic
{

    public class SubPageInfo
    {
        public string Name { get; init; }
        public Type Component { get; init; }
        public string Svg { get; init; }
        public string Tooltip { get; init; }
        public bool SplitPage { get; init; }


        public static readonly SubPageInfo[] SubPages = [
             new SubPageInfo {
                 Name = "Merilno mesto",
                 SplitPage = true,
                 Component = typeof(MeteringPointSettingsPage),
                 Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-device-ssd\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M4.75 4a.75.75 0 0 0-.75.75v3.5c0 .414.336.75.75.75h6.5a.75.75 0 0 0 .75-.75v-3.5a.75.75 0 0 0-.75-.75zM5 8V5h6v3zm0-5.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m7 0a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0M4.5 11a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1m7 0a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1\"/>\r\n  <path d=\"M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2zm11 12V2a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1v-2a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2a1 1 0 0 0 1-1m-7.25 1v-2H5v2zm1.75 0v-2h-.75v2zm1.75 0v-2H8.5v2zM11 13h-.75v2H11z\"/>\r\n</svg>"
             },
             new SubPageInfo {
                 Name = "Pretekla poraba",
                 SplitPage = false,
                 Component = typeof(DataLoadingComponent),
                 Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-clipboard-data-fill\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M6.5 0A1.5 1.5 0 0 0 5 1.5v1A1.5 1.5 0 0 0 6.5 4h3A1.5 1.5 0 0 0 11 2.5v-1A1.5 1.5 0 0 0 9.5 0zm3 1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5v-1a.5.5 0 0 1 .5-.5z\"/>\r\n  <path d=\"M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1A2.5 2.5 0 0 1 9.5 5h-3A2.5 2.5 0 0 1 4 2.5zM10 8a1 1 0 1 1 2 0v5a1 1 0 1 1-2 0zm-6 4a1 1 0 1 1 2 0v1a1 1 0 1 1-2 0zm4-3a1 1 0 0 1 1 1v3a1 1 0 1 1-2 0v-3a1 1 0 0 1 1-1\"/>\r\n</svg>"
             },
             new SubPageInfo {
                 Name = "Dogovorjene moči",
                 SplitPage = true,
                 Component = typeof(BlocksPowerPage),
                 Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-lightning-charge-fill\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M11.251.068a.5.5 0 0 1 .227.58L9.677 6.5H13a.5.5 0 0 1 .364.843l-8 8.5a.5.5 0 0 1-.842-.49L6.323 9.5H3a.5.5 0 0 1-.364-.843l8-8.5a.5.5 0 0 1 .615-.09z\"/>\r\n</svg>"
             },
            new SubPageInfo{
                Name="Sočna elektrarna",
                SplitPage = false,
                Component=typeof(SolarPanelsSettings),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-sun\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M8 11a3 3 0 1 1 0-6 3 3 0 0 1 0 6m0 1a4 4 0 1 0 0-8 4 4 0 0 0 0 8M8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0m0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13m8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5M3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8m10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0m-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0m9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707M4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708\"/>\r\n</svg>"
            },
            new SubPageInfo{
                Name="Baterije",
                SplitPage = false,
                Component=typeof(BatteriesSettings),
                Svg="<?xml version=\"1.0\" encoding=\"UTF-8\"?><svg width=\"24\" height=\"24\" viewBox=\"0 0 48 48\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><rect x=\"14\" y=\"44\" width=\"36\" height=\"20\" rx=\"2\" transform=\"rotate(-90 14 44)\" fill=\"none\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M20 6L20 4C20 2.89543 20.8954 2 22 2L26 2C27.1046 2 28 2.89543 28 4L28 6L20 6Z\" fill=\"#fff\"/><path d=\"M25 18L20 28L28 26L23 36\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>"
            },
            new SubPageInfo{
                Name="Električni avtomobil",
                SplitPage = false,
                Component=typeof(CarSettings),
                Svg="<?xml version=\"1.0\" encoding=\"UTF-8\"?><svg width=\"24\" height=\"24\" viewBox=\"0 0 48 48\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M13.5 32C14.8807 32 16 30.8807 16 29.5C16 28.1193 14.8807 27 13.5 27C12.1193 27 11 28.1193 11 29.5C11 30.8807 12.1193 32 13.5 32Z\" fill=\"#fff\"/><path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M34.5 32C35.8807 32 37 30.8807 37 29.5C37 28.1193 35.8807 27 34.5 27C33.1193 27 32 28.1193 32 29.5C32 30.8807 33.1193 32 34.5 32Z\" fill=\"#fff\"/><path d=\"M7 37C5.34315 37 4 35.6569 4 34L4 24.7097C4 22.4363 5.28486 20.3581 7.3186 19.3422L8.00053 19.0015L10.3105 9.09194C10.7326 7.28117 12.3467 6 14.206 6L33.8943 6C35.7675 6 37.3899 7.29998 37.7981 9.12816L40.0031 19.0015L40.6834 19.3416C42.716 20.358 44 22.4355 44 24.7081V34C44 35.6569 42.6569 37 41 37H39.0031V38C39.0031 40.2091 37.2107 42 35.0016 42C32.7924 42 31 40.2091 31 38V37H17V38.0003C17 40.2093 15.2093 42 13.0003 42C10.7913 42 9.00053 40.2093 9.00053 38.0003V37H7Z\" fill=\"none\" stroke=\"#fff\" stroke-width=\"4\" stroke-linejoin=\"round\"/><path d=\"M14 22H34L32.348 14.5661C32.1447 13.6511 31.3331 13 30.3957 13L17.6043 13C16.6669 13 15.8553 13.6511 15.652 14.5661L14 22Z\" fill=\"none\" stroke=\"#fff\" stroke-width=\"4\" stroke-linejoin=\"round\"/></svg>"
            },
            new SubPageInfo{
                Name="Toplotna črpalka",
                SplitPage = false,
                Component=typeof(HeatPumpSettings),
                Svg="<?xml version=\"1.0\" encoding=\"UTF-8\"?><svg width=\"24\" height=\"24\" viewBox=\"0 0 48 48\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M24 4V44\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M6.72461 14L41.3656 34\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M6.71923 33.9773L41.2814 14.0228\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M12 10L15 19L6 21\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M6 27L15 29L12 38\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M36 10L33 19L42 21\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M42 27L33 29L36 38\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M18 7L24 13L30 7\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/><path d=\"M18 41L24 35L30 41\" stroke=\"#fff\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>"
            },
            new SubPageInfo{
                Name="Cena energije",
                SplitPage = true,
                Component=typeof(EnergyPricePage),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-currency-euro\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M4 9.42h1.063C5.4 12.323 7.317 14 10.34 14c.622 0 1.167-.068 1.659-.185v-1.3c-.484.119-1.045.17-1.659.17-2.1 0-3.455-1.198-3.775-3.264h4.017v-.928H6.497v-.936q-.002-.165.008-.329h4.078v-.927H6.618c.388-1.898 1.719-2.985 3.723-2.985.614 0 1.175.05 1.659.177V2.194A6.6 6.6 0 0 0 10.341 2c-2.928 0-4.82 1.569-5.244 4.3H4v.928h1.01v1.265H4v.928z\"/>\r\n</svg>"
            },
            new SubPageInfo{
                Name="Nastavitve",
                SplitPage = true,
                Component=typeof(CalculationsSettingsPage),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-gear\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492M5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0\"/>\r\n  <path d=\"M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0l-.094.319a.873.873 0 0 1-1.255.52l-.292-.16c-1.64-.892-3.433.902-2.54 2.541l.159.292a.873.873 0 0 1-.52 1.255l-.319.094c-1.79.527-1.79 3.065 0 3.592l.319.094a.873.873 0 0 1 .52 1.255l-.16.292c-.892 1.64.901 3.434 2.541 2.54l.292-.159a.873.873 0 0 1 1.255.52l.094.319c.527 1.79 3.065 1.79 3.592 0l.094-.319a.873.873 0 0 1 1.255-.52l.292.16c1.64.893 3.434-.902 2.54-2.541l-.159-.292a.873.873 0 0 1 .52-1.255l.319-.094c1.79-.527 1.79-3.065 0-3.592l-.319-.094a.873.873 0 0 1-.52-1.255l.16-.292c.893-1.64-.902-3.433-2.541-2.54l-.292.159a.873.873 0 0 1-1.255-.52zm-2.633.283c.246-.835 1.428-.835 1.674 0l.094.319a1.873 1.873 0 0 0 2.693 1.115l.291-.16c.764-.415 1.6.42 1.184 1.185l-.159.292a1.873 1.873 0 0 0 1.116 2.692l.318.094c.835.246.835 1.428 0 1.674l-.319.094a1.873 1.873 0 0 0-1.115 2.693l.16.291c.415.764-.42 1.6-1.185 1.184l-.291-.159a1.873 1.873 0 0 0-2.693 1.116l-.094.318c-.246.835-1.428.835-1.674 0l-.094-.319a1.873 1.873 0 0 0-2.692-1.115l-.292.16c-.764.415-1.6-.42-1.184-1.185l.159-.291A1.873 1.873 0 0 0 1.945 8.93l-.319-.094c-.835-.246-.835-1.428 0-1.674l.319-.094A1.873 1.873 0 0 0 3.06 4.377l-.16-.292c-.415-.764.42-1.6 1.185-1.184l.292.159a1.873 1.873 0 0 0 2.692-1.115z\"/>\r\n</svg>"
            },
            new SubPageInfo {
                Name = "Kalkulator",
                SplitPage = false,
                Component = typeof(Kalkulator),
                Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-calculator\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M12 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1zM4 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2z\"/>\r\n  <path d=\"M4 2.5a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-.5.5h-7a.5.5 0 0 1-.5-.5zm0 4a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm3-6a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm3-6a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v4a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5z\"/>\r\n</svg>"
            },

        ];
    }

    public partial class ViewModel : ObservableObject
    {
        private OmrezninaReport? leftReport;
        private OmrezninaReport? rightReport;
        private Dictionary<(int Year, int Month), List<RawUsage15Min>>? rawUsages;

        [ObservableProperty]
        private SubPageInfo currentPage = SubPageInfo.SubPages[0];

        public ViewModel()
        {
            LeftCalculationOptions.PropertyChanged += (s, e) => this.OnPropertyChanged(e);
            RightCalculationOptions.PropertyChanged += (s, e) => this.OnPropertyChanged(e);
            GlobalCalculationOptions.PropertyChanged += (s, e) => this.OnPropertyChanged(e);
            FocusedCalculationOptions = LeftCalculationOptions;
        }

        public GlobalCalculationOptions GlobalCalculationOptions { get; } = new GlobalCalculationOptions();

        public List<CalculationOptions> AllCalculationOptions = [];

        public CalculationOptions LeftCalculationOptions { get; set; } = new CalculationOptions();
        public CalculationOptions RightCalculationOptions { get; set; } = new CalculationOptions();
        public CalculationOptions FocusedCalculationOptions { get; set; }
        [ObservableProperty]
        private bool has15MinuteData = true;

        public async Task LoadCSVs(List<Stream> csvStreams)
        {
            rawUsages = await OmrezninaReport.ParseRawUsages(csvStreams);
            GlobalCalculationOptions.Has15MinuteData = true;
        }

        public ManuallyEnteredMonthlyConsumption[] MonthsEnergyUsageInKWh { get; } = Enumerable.Range(0, 12).Select(i => new ManuallyEnteredMonthlyConsumption()).ToArray();

        public MonthsViewModel MonthsViewModel { get; } = new();
        public AllCategoriesViewModel AllCategoriesViewModel { get; } = new();
        public DaysViewModel DaysViewModel { get; } = new();
        public FifteenMinutesViewModel FifteenMinutesViewModel { get; } = new();

        public void UpdateGraphs(bool useCached = false)
        {
            if (!LeftCalculationOptions.EnteredCalculatorOnce || !RightCalculationOptions.EnteredCalculatorOnce)
            {
                return;
            }
            if (!useCached || leftReport == null)
                leftReport = new OmrezninaReport(LeftCalculationOptions, rawUsages, MonthsEnergyUsageInKWh);
            if (!useCached || rightReport == null)
                rightReport = new OmrezninaReport(RightCalculationOptions, rawUsages, MonthsEnergyUsageInKWh);
            MonthsViewModel.Update(leftReport, rightReport);
            AllCategoriesViewModel.Update(leftReport, rightReport);
            DaysViewModel.Update(leftReport.MonthlyReports[MonthsViewModel.SelectMonth], rightReport.MonthlyReports[MonthsViewModel.SelectMonth]);
            FifteenMinutesViewModel.Update(RightCalculationOptions, rightReport.MonthlyReports[MonthsViewModel.SelectMonth].DailyReports.FirstOrDefault(d => d.Value.IsSelected).Value);
        }

        public void PreviousPage()
        {
            var currentPageIndex = Array.IndexOf(SubPageInfo.SubPages, CurrentPage);
            CurrentPage = SubPageInfo.SubPages[currentPageIndex - 1];
        }

        public void NextPage()
        {
            if (FocusedCalculationOptions.EnteredCalculatorOnce)
            {
                CurrentPage = SubPageInfo.SubPages.Single(p => p.Component == typeof(Kalkulator));
                UpdateGraphs();
            }
            else
            {
                var currentPageIndex = Array.IndexOf(SubPageInfo.SubPages, CurrentPage);
                var nextCurrentPage = SubPageInfo.SubPages[currentPageIndex + 1];
                CurrentPage = nextCurrentPage;
                if (CurrentPage.Component == typeof(Kalkulator))
                {
                    //FocusedCalculationOptions.EnteredCalculatorOnce = true;
                    LeftCalculationOptions.EnteredCalculatorOnce = true;
                    RightCalculationOptions.EnteredCalculatorOnce = true;
                    UpdateGraphs();
                }
            }
        }
    }
}
