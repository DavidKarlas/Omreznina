using Omreznina.Client.Logic;
using System.ComponentModel;

namespace Omreznina.Logic
{
    public class ViewModel
    {
        public CalculationOptions CalculationOptions { get; } = new CalculationOptions();

        private Dictionary<(int Year, int Month), List<RawUsage15Min>>? rawUsages;

        public async Task LoadCSVs(List<Stream> csvStreams)
        {
            rawUsages = await OmrezninaReport.ParseRawUsages(csvStreams);
            CalculationOptions.No15MinuteData = false;
            ConfirmedData = true;
            UpdateGraphs();
        }

        public decimal[] MonthsEnergyUsageInKWh { get; } = new decimal[12];


        public bool ConfirmedSettings { get; set; } = false;
        public bool ConfirmedData { get; set; } = false;
        public bool ConfirmedPowersPerBlock { get; set; } = false;
        public bool HasCsvData => rawUsages is not null;
        public bool HasMonthsEnergyData => !MonthsEnergyUsageInKWh.Any(x => x == 0);

        public MonthsViewModel MonthsViewModel { get; } = new();
        public AllCategoriesViewModel AllCategoriesViewModel { get; } = new();
        public DaysViewModel DaysViewModel { get; } = new();
        public FifteenMinutesViewModel FifteenMinutesViewModel { get; } = new();
        OmrezninaReport mainReport;

        public void UpdateGraphs(bool useCached = false)
        {
            if (!useCached)
                mainReport = new OmrezninaReport(CalculationOptions, rawUsages, MonthsEnergyUsageInKWh);
            MonthsViewModel.Update(mainReport);
            AllCategoriesViewModel.Update(mainReport);
            DaysViewModel.Update(mainReport.MonthlyReports[MonthsViewModel.SelectMonth]);
            FifteenMinutesViewModel.Update(CalculationOptions, DaysViewModel.SelectedDay);
        }
    }
}
