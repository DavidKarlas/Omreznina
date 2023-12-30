using Omreznina.Client.Logic;

namespace Omreznina.Logic
{
    public class ViewModel
    {
        public CalculationOptions CalculationOptions { get; } = new CalculationOptions();

        private Dictionary<(int Year, int Month), List<RawUsage15Min>>? rawUsages;

        public async Task LoadCSVs(List<Stream> csvStreams)
        {
            rawUsages = await OmrezninaReport.ParseRawUsages(csvStreams);
            UpdateGraphs();
        }

        public MonthsViewModel MonthsViewModel { get; } = new();
        public AllCategoriesViewModel AllCategoriesViewModel { get; } = new();
        public DaysViewModel DaysViewModel { get; } = new();
        public FifteenMinutesViewModel FifteenMinutesViewModel { get; } = new();
        OmrezninaReport mainReport;
        public void UpdateGraphs(bool useCached = false)
        {
            if (!useCached)
                mainReport = new OmrezninaReport(CalculationOptions, rawUsages);
            MonthsViewModel.Update(mainReport);
            AllCategoriesViewModel.Update(mainReport);
            DaysViewModel.Update(mainReport.MonthlyReports[MonthsViewModel.SelectMonth]);
            FifteenMinutesViewModel.Update(CalculationOptions, DaysViewModel.SelectedDay);
        }
    }
}
