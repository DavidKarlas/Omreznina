using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Omreznina.Client.Logic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using LiveChartsCore.Measure;
using Omreznina.Client.Pages;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace Omreznina.Logic
{
    public class DaysViewModel
    {
        private readonly ObservableCollection<DayReport> days = new();

        public int DataLabelsTotalSize { get; set; } = 12;
        public int DataLabelsSize { get; set; } = 12;
        private readonly SolidColorPaint DataLabelPaint = "#e5e5e5".ToPaint();
        private readonly SolidColorPaint TotalDataLabelPaint = "#303030".ToPaint();

        public ISeries[] Series { get; }
        public Axis[] XAxis { get; }
        public Axis[] YAxis { get; }
        public bool IsVisible { get; set; } = false;
        public LabelVisual Title { get; set; } =
            new LabelVisual {
                Text = "Izberi mesec",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
        public DaysViewModel()
        {
            Series = [
               new ColumnSeries<DayReport>
               {
                    IsHoverable = false, // disables the series from the tooltips // mark
                    Values = days,
                    Mapping = (day, index) => new LiveChartsCore.Kernel.Coordinate(day.Day.Day, day.IsSelected?1000000:0),
                    Stroke = null,
                    Fill = new SolidColorPaint( "#2196f3".ToPaint().Color.WithAlpha(170),0),
                    IgnoresBarPosition = true,
                    MaxBarWidth=int.MaxValue,
                    IsVisibleAtLegend = false
                },
                CreateStackedColumn("Dogovorjeno", 1, UIHelper.AgreedPowerColor, days, (day, index) => new LiveChartsCore.Kernel.Coordinate(day.Day.Day, (double)day.AgreedPowerPrice)),
                CreateStackedColumn("Energija", 1, UIHelper.EnergyTransferColor, days, (day, index) => new LiveChartsCore.Kernel.Coordinate(day.Day.Day, (double)day.EnergyPrice)),
                CreateStackedColumn("Prekoračitev", 1, UIHelper.OverdraftColor, days, (day, index) => new LiveChartsCore.Kernel.Coordinate(day.Day.Day, (double)day.OverdraftPowerPrice)),
                new StackedColumnSeries<DayReport>
                {
                    IsVisibleAtLegend = false,
                    Name="Suma",
                    Values = days,
                    Mapping = (day, index) => new LiveChartsCore.Kernel.Coordinate(day.Day.Day, 0),
                    Stroke = null,
                    StackGroup = 1,
                    DataLabelsSize=DataLabelsTotalSize,
                    DataLabelsFormatter=point => ((decimal)point.StackedValue.Total).ToEuro(),
                    DataLabelsPaint =TotalDataLabelPaint,
                    DataLabelsPosition = DataLabelsPosition.Top,
                    YToolTipLabelFormatter = point => ((decimal)point.StackedValue.Total).ToEuro(),
                    MaxBarWidth = 55
                }
            ];
            XAxis = [new Axis {
                Labeler = value => ((int)value).ToString(),
            }];
            YAxis = [new Axis {
                MinLimit = 0,
                Labeler = value => ((decimal)value).ToEuroPreferFullNumber()
            }];
        }

        private StackedColumnSeries<DayReport> CreateStackedColumn(string name, int stackGroup, SolidColorPaint color, IEnumerable<DayReport> values, Func<DayReport, int, LiveChartsCore.Kernel.Coordinate> mapping)
        {
            return new StackedColumnSeries<DayReport> {
                Name = name,
                Values = values,
                Stroke = null,
                StackGroup = stackGroup,
                DataLabelsSize = 11,
                Mapping = mapping,
                DataLabelsFormatter = point => {
                    if (YAxis[0].VisibleDataBounds.Max == 0)
                        return "";
                    if (((double)point.Coordinate.PrimaryValue / YAxis[0].VisibleDataBounds.Max) < 0.03)
                        return "";
                    return point.Coordinate.PrimaryValue.ToEuro();
                },
                DataLabelsPaint = new SolidColorPaint(new SKColor(240, 240, 240)),
                DataLabelsPosition = DataLabelsPosition.Middle,
                YToolTipLabelFormatter = point => point.Coordinate.PrimaryValue.ToEuro(),
                XToolTipLabelFormatter = point => point.Model?.Day.ToString("dddd d.MMM") ?? "",
                Fill = color
            };
        }

        public void SelectDay(DayReport dayReport)
        {
            foreach (var item in previouslySelectedMonth.DailyReports)
            {
                item.Value.IsSelected = false;
            }
            dayReport.IsSelected = true;
        }

        private MonthlyReport previouslySelectedMonth;

        public void Update(MonthlyReport monthReport)
        {
            var allDays = monthReport.DailyReports.Values.OrderBy(d => d.Index).ToArray();
            if (allDays.Length == 0)
            {
                IsVisible = false;
                return;
            }
            if (!IsVisible)
            {
                IsVisible = true;
            }

            Title.Text = $"{UIHelper.MonthConverter(monthReport.Month.Month, false)} - {monthReport.Month.Year}";
            if (!allDays.Any(d => d.IsSelected))
            {
                allDays
                    .OrderByDescending(d => d.OverdraftPowerPrice)
                    .ThenByDescending(d => d.EnergyPrice + d.AgreedPowerPrice)
                    .ThenBy(d => d.Index)
                    .First().IsSelected = true;
            }
            days.SyncCollections(allDays);
            var maxY = (double)allDays.Max(x => x.EnergyPrice + x.OverdraftPowerPrice + x.AgreedPowerPrice);
            var roundedMaxY = Math.Round(maxY * 1.25) + 1;
            if (YAxis[0].MaxLimit != roundedMaxY)
            {
                YAxis[0].MaxLimit = roundedMaxY;
            }
            previouslySelectedMonth = monthReport;
        }
    }
}
