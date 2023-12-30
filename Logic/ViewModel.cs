using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using Omreznina.Client.Logic;
using System.Diagnostics;

namespace Omreznina.Logic
{
    public class ViewModel
    {
        public CalculationOptions CalculationOptions { get; } = new CalculationOptions([7.5M, 8, 9, 10, 11], 2025, false, 10);

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
            if (rawUsages == null)
                return;
            if (!useCached)
                mainReport = new OmrezninaReport(CalculationOptions, rawUsages);
            MonthsViewModel.Update(mainReport);
            AllCategoriesViewModel.Update(mainReport);
            DaysViewModel.Update(mainReport.MonthlyReports[MonthsViewModel.SelectMonth]);
            FifteenMinutesViewModel.Update(CalculationOptions, DaysViewModel.SelectedDay);
        }
    }
}
