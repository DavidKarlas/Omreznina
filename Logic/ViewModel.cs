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


        public static readonly SubPageInfo[] SubPages = [
             new SubPageInfo {
                 Name = "Merilno mesto",
                 Component = typeof(MeteringPointSettingsPage),
                 Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-device-ssd\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M4.75 4a.75.75 0 0 0-.75.75v3.5c0 .414.336.75.75.75h6.5a.75.75 0 0 0 .75-.75v-3.5a.75.75 0 0 0-.75-.75zM5 8V5h6v3zm0-5.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0m7 0a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0M4.5 11a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1m7 0a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1\"/>\r\n  <path d=\"M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2zm11 12V2a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1v-2a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2a1 1 0 0 0 1-1m-7.25 1v-2H5v2zm1.75 0v-2h-.75v2zm1.75 0v-2H8.5v2zM11 13h-.75v2H11z\"/>\r\n</svg>"
             },
             new SubPageInfo {
                 Name = "Pretekla poraba",
                 Component = typeof(DataLoadingComponent),
                 Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-clipboard-data-fill\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M6.5 0A1.5 1.5 0 0 0 5 1.5v1A1.5 1.5 0 0 0 6.5 4h3A1.5 1.5 0 0 0 11 2.5v-1A1.5 1.5 0 0 0 9.5 0zm3 1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5v-1a.5.5 0 0 1 .5-.5z\"/>\r\n  <path d=\"M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1A2.5 2.5 0 0 1 9.5 5h-3A2.5 2.5 0 0 1 4 2.5zM10 8a1 1 0 1 1 2 0v5a1 1 0 1 1-2 0zm-6 4a1 1 0 1 1 2 0v1a1 1 0 1 1-2 0zm4-3a1 1 0 0 1 1 1v3a1 1 0 1 1-2 0v-3a1 1 0 0 1 1-1\"/>\r\n</svg>"
             },
             new SubPageInfo {
                 Name = "Dogovorjene moči",
                 Component = typeof(BlocksPowerPage),
                 Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-lightning-charge-fill\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M11.251.068a.5.5 0 0 1 .227.58L9.677 6.5H13a.5.5 0 0 1 .364.843l-8 8.5a.5.5 0 0 1-.842-.49L6.323 9.5H3a.5.5 0 0 1-.364-.843l8-8.5a.5.5 0 0 1 .615-.09z\"/>\r\n</svg>"
             },
            new SubPageInfo{
                Name="Sočna elektrarna",
                Component=typeof(SolarPanelsSettings),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-sun\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M8 11a3 3 0 1 1 0-6 3 3 0 0 1 0 6m0 1a4 4 0 1 0 0-8 4 4 0 0 0 0 8M8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0m0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13m8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5M3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8m10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0m-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0m9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707M4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708\"/>\r\n</svg>"
            },
            new SubPageInfo{
                Name="Cena energije",
                Component=typeof(EnergyPricePage),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-currency-euro\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M4 9.42h1.063C5.4 12.323 7.317 14 10.34 14c.622 0 1.167-.068 1.659-.185v-1.3c-.484.119-1.045.17-1.659.17-2.1 0-3.455-1.198-3.775-3.264h4.017v-.928H6.497v-.936q-.002-.165.008-.329h4.078v-.927H6.618c.388-1.898 1.719-2.985 3.723-2.985.614 0 1.175.05 1.659.177V2.194A6.6 6.6 0 0 0 10.341 2c-2.928 0-4.82 1.569-5.244 4.3H4v.928h1.01v1.265H4v.928z\"/>\r\n</svg>"
            },
            new SubPageInfo{
                Name="Nastavitve",
                Component=typeof(CalculationsSettingsPage),
                Svg="<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-gear\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M8 4.754a3.246 3.246 0 1 0 0 6.492 3.246 3.246 0 0 0 0-6.492M5.754 8a2.246 2.246 0 1 1 4.492 0 2.246 2.246 0 0 1-4.492 0\"/>\r\n  <path d=\"M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0l-.094.319a.873.873 0 0 1-1.255.52l-.292-.16c-1.64-.892-3.433.902-2.54 2.541l.159.292a.873.873 0 0 1-.52 1.255l-.319.094c-1.79.527-1.79 3.065 0 3.592l.319.094a.873.873 0 0 1 .52 1.255l-.16.292c-.892 1.64.901 3.434 2.541 2.54l.292-.159a.873.873 0 0 1 1.255.52l.094.319c.527 1.79 3.065 1.79 3.592 0l.094-.319a.873.873 0 0 1 1.255-.52l.292.16c1.64.893 3.434-.902 2.54-2.541l-.159-.292a.873.873 0 0 1 .52-1.255l.319-.094c1.79-.527 1.79-3.065 0-3.592l-.319-.094a.873.873 0 0 1-.52-1.255l.16-.292c.893-1.64-.902-3.433-2.541-2.54l-.292.159a.873.873 0 0 1-1.255-.52zm-2.633.283c.246-.835 1.428-.835 1.674 0l.094.319a1.873 1.873 0 0 0 2.693 1.115l.291-.16c.764-.415 1.6.42 1.184 1.185l-.159.292a1.873 1.873 0 0 0 1.116 2.692l.318.094c.835.246.835 1.428 0 1.674l-.319.094a1.873 1.873 0 0 0-1.115 2.693l.16.291c.415.764-.42 1.6-1.185 1.184l-.291-.159a1.873 1.873 0 0 0-2.693 1.116l-.094.318c-.246.835-1.428.835-1.674 0l-.094-.319a1.873 1.873 0 0 0-2.692-1.115l-.292.16c-.764.415-1.6-.42-1.184-1.185l.159-.291A1.873 1.873 0 0 0 1.945 8.93l-.319-.094c-.835-.246-.835-1.428 0-1.674l.319-.094A1.873 1.873 0 0 0 3.06 4.377l-.16-.292c-.415-.764.42-1.6 1.185-1.184l.292.159a1.873 1.873 0 0 0 2.692-1.115z\"/>\r\n</svg>"
            },
             new SubPageInfo {
                 Name = "Kalkulator",
                 Component = typeof(Kalkulator),
                 Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-calculator\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M12 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1zM4 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2z\"/>\r\n  <path d=\"M4 2.5a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-.5.5h-7a.5.5 0 0 1-.5-.5zm0 4a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm3-6a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm3-6a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5zm0 3a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v4a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5z\"/>\r\n</svg>"
             },

        ];
    }

    public partial class ViewModel : ObservableObject
    {
        private OmrezninaReport? mainReport;
        private Dictionary<(int Year, int Month), List<RawUsage15Min>>? rawUsages;

        [ObservableProperty]
        private SubPageInfo currentPage = SubPageInfo.SubPages[0];

        public ViewModel()
        {
            CalculationOptions.PropertyChanged += (s, e) => this.OnPropertyChanged(e);
        }

        public CalculationOptions CalculationOptions { get; } = new CalculationOptions();

        public async Task LoadCSVs(List<Stream> csvStreams)
        {
            rawUsages = await OmrezninaReport.ParseRawUsages(csvStreams);
            CalculationOptions.Has15MinuteData = true;
        }

        public ManuallyEnteredMonthlyConsumption[] MonthsEnergyUsageInKWh { get; } = Enumerable.Range(0, 12).Select(i => new ManuallyEnteredMonthlyConsumption()).ToArray();

        public MonthsViewModel MonthsViewModel { get; } = new();
        public AllCategoriesViewModel AllCategoriesViewModel { get; } = new();
        public DaysViewModel DaysViewModel { get; } = new();
        public FifteenMinutesViewModel FifteenMinutesViewModel { get; } = new();

        public void UpdateGraphs(bool useCached = false)
        {
            if (!EnteredCalculatorOnce)
            {
                return;
            }
            if (!useCached || mainReport == null)
                mainReport = new OmrezninaReport(CalculationOptions, rawUsages, MonthsEnergyUsageInKWh);
            MonthsViewModel.Update(mainReport);
            AllCategoriesViewModel.Update(mainReport);
            DaysViewModel.Update(mainReport.MonthlyReports[MonthsViewModel.SelectMonth]);
            FifteenMinutesViewModel.Update(CalculationOptions, mainReport.MonthlyReports[MonthsViewModel.SelectMonth].DailyReports.FirstOrDefault(d => d.Value.IsSelected).Value);
        }

        [ObservableProperty]
        private bool enteredCalculatorOnce = false;


        public void PreviousPage()
        {
            var currentPageIndex = Array.IndexOf(SubPageInfo.SubPages, CurrentPage);
            CurrentPage = SubPageInfo.SubPages[currentPageIndex - 1];
        }

        public void NextPage()
        {
            if (EnteredCalculatorOnce)
            {
                CurrentPage = SubPageInfo.SubPages.Single(p => p.Component == typeof(Kalkulator));
                UpdateGraphs();
            }
            else
            {
                var currentPageIndex = Array.IndexOf(SubPageInfo.SubPages, CurrentPage);
                CurrentPage = SubPageInfo.SubPages[currentPageIndex + 1];
                if (CurrentPage.Component == typeof(Kalkulator))
                {
                    EnteredCalculatorOnce = true;
                    UpdateGraphs();
                }
            }
        }
    }
}
