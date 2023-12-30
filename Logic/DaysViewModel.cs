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
        private readonly ObservableCollection<string> days = new();
        private readonly ObservableCollection<decimal> agreedPowerPrice = new();
        private readonly ObservableCollection<decimal> energyPrice = new();
        private readonly ObservableCollection<decimal> overdraftPrice = new();
        private readonly ObservableCollection<decimal> zeros = new();
        private readonly ObservableCollection<decimal> selected = new();

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
               new ColumnSeries<decimal>
               {
                    IsHoverable = false, // disables the series from the tooltips // mark
                    Values = selected,
                    Stroke = null,
            Fill =new SolidColorPaint( "#2196f3".ToPaint().Color.WithAlpha(170),0),
                    IgnoresBarPosition = true,
                    MaxBarWidth=int.MaxValue,
                    IsVisibleAtLegend = false
                },
                CreateStackedColumn("Dogovorjeno", 1, UIHelper.AgreedPowerColor, agreedPowerPrice),
                        CreateStackedColumn("Energija", 1, UIHelper.EnergyTransferColor, energyPrice),
                        CreateStackedColumn("Prekoračitev", 1, UIHelper.OverdraftColor, overdraftPrice),
                        new StackedColumnSeries<decimal>
                        {
                            IsVisibleAtLegend = false,
                            Name="Suma",
                            Values = zeros,
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
                Labels = days
            }];
            YAxis = [new Axis {
                MinLimit = 0,
                Labeler = value => ((decimal)value).ToEuroPreferFullNumber()
            }];
        }

        private StackedColumnSeries<decimal> CreateStackedColumn(string name, int stackGroup, SolidColorPaint color, IEnumerable<decimal> values)
        {
            return new StackedColumnSeries<decimal> {
                Name = name,
                Values = values,
                Stroke = null,
                StackGroup = stackGroup,
                DataLabelsSize = 11,
                DataLabelsFormatter = point => {
                    if (YAxis[0].VisibleDataBounds.Max == 0)
                        return "";
                    if (((double)point.Model / YAxis[0].VisibleDataBounds.Max) < 0.03)
                        return "";
                    return point.Model.ToEuro();
                },
                DataLabelsPaint = new SolidColorPaint(new SKColor(240, 240, 240)),
                DataLabelsPosition = DataLabelsPosition.Middle,
                YToolTipLabelFormatter = point => point.Model.ToEuro(),
                Fill = color
            };
        }

        public void SelectDay(int index)
        {
            foreach (var day in previouslySelectedMonth.DailyReports)
            {
                if (index == day.Value.Index)
                {
                    SelectedDay = day.Value;
                    return;
                }
            }
        }

        private DayReport? selectedDay = null;
        private MonthlyReport previouslySelectedMonth;
        public DayReport? SelectedDay
        {
            get
            {
                return selectedDay;
            }
            set
            {
                var newArray = new decimal[days.Count];
                selectedDay = value;
                if (selectedDay != null)
                    newArray[selectedDay.Index] = (decimal)YAxis[0].MaxLimit!;
                selected.SyncCollections(newArray);
            }
        }

        public void Update(MonthlyReport monthReport)
        {
            var allDays = monthReport.DailyReports.Values.OrderBy(d => d.Index).ToArray();
            if (allDays.Length == 0)
            {
                SelectedDay = null;
                IsVisible = false;
                return;
            }
            if(!IsVisible)
            {
                IsVisible = true;
            }
            days.SyncCollections(allDays.Select(d => d.Day.Day.ToString()).ToArray());
            var maxY = (double)allDays.Max(x => x.EnergyPrice + x.OverdraftPowerPrice + x.AgreedPowerPrice);
            var roundedMaxY = Math.Round(maxY * 1.25) + 1;
            if (YAxis[0].MaxLimit != roundedMaxY)
            {
                YAxis[0].MaxLimit = roundedMaxY;
            }
            if (monthReport.Month != previouslySelectedMonth?.Month || selectedDay == null || selectedDay.Index >= days.Count)
            {
                SelectedDay = allDays
                    .OrderByDescending(d => d.OverdraftPowerPrice)
                    .ThenByDescending(d => d.EnergyPrice + d.AgreedPowerPrice)
                    .ThenBy(d => d.Index)
                    .First();
            }
            else
            {
                SelectedDay = monthReport.DailyReports[SelectedDay.Day];
            }
            previouslySelectedMonth = monthReport;
            agreedPowerPrice.SyncCollections(allDays.Select(d => d.AgreedPowerPrice).ToArray());
            energyPrice.SyncCollections(allDays.Select(d => d.EnergyPrice).ToArray());
            overdraftPrice.SyncCollections(allDays.Select(d => d.OverdraftPowerPrice).ToArray());
            zeros.SyncCollections(new decimal[days.Count]);
        }
    }
}
