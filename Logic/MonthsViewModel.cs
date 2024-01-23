using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Omreznina.Client.Logic;
using Omreznina.Client.Pages;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Omreznina.Logic
{
    public class MonthsViewModel
    {
        private readonly ObservableCollection<string> months = new();
        private readonly ObservableCollection<decimal> oldFixedPrice = new();
        private readonly ObservableCollection<decimal> oldEnergyTransportPrice = new();
        private readonly ObservableCollection<decimal> energyPrice = new();
        private readonly ObservableCollection<decimal> agreedPowerPrice = new();
        private readonly ObservableCollection<decimal> energyTransferPrice = new();
        private readonly ObservableCollection<decimal> overdraftPrice = new();
        private readonly ObservableCollection<decimal> zeros = new();
        private readonly ObservableCollection<decimal> selected = new();

        public ISeries[] Series { get; private set; } = [];
        public Axis[] XAxis { get; private set; } = [];

        public LabelVisual Title { get; set; } =
            new LabelVisual {
                Text = "Primerjava po mesecih",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public Axis[] YAxis { get; private set; } = [];
        public int DataLabelsTotalSize { get; set; } = 12;
        public int DataLabelsSize { get; set; } = 12;
        private readonly SolidColorPaint DataLabelPaint = "#e5e5e5".ToPaint();
        private readonly SolidColorPaint TotalDataLabelPaint = "#303030".ToPaint();
        private int? selectMonth = null;

        public MonthsViewModel()
        {
            XAxis = [new Axis {
                Labels = months
            }];
            YAxis = [new Axis {
                MinLimit = 0,
                Labeler = value => ((decimal)value).ToEuroPreferFullNumber()
            }];
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
                    CreateStackedColumn("Star obračun: Omrež. Moč", 0, UIHelper.AgreedPowerColor, oldFixedPrice, true),
                    CreateStackedColumn("Star obračun: Omrež. Poraba", 0, UIHelper.EnergyTransferColor, oldEnergyTransportPrice, true),
                    CreateStackedColumn("Energija", 0, UIHelper.EnergyColor, energyPrice, true),
                    new StackedColumnSeries<decimal>
                    {
                        IsVisibleAtLegend = false,
                        Name="Suma",
                        Values = zeros,
                        Stroke = null,
                        StackGroup = 0,
                        DataLabelsSize = DataLabelsTotalSize,
                        DataLabelsFormatter=point => ((decimal)point.StackedValue.Total).ToEuro(),
                        DataLabelsPaint =TotalDataLabelPaint,
                        DataLabelsPosition = DataLabelsPosition.Top,
                        YToolTipLabelFormatter = point => ((decimal)point.StackedValue.Total).ToEuro(),
                        MaxBarWidth = 55
                    },
                    CreateStackedColumn("Omrež. moč", 1, UIHelper.AgreedPowerColor, agreedPowerPrice),
                    CreateStackedColumn("Omrež. poraba", 1, UIHelper.EnergyTransferColor, energyTransferPrice),
                    CreateStackedColumn("Energija", 1, UIHelper.EnergyColor, energyPrice),
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
        }

        private StackedColumnSeries<decimal> CreateStackedColumn(string name, int stackGroup, SolidColorPaint color, IEnumerable<decimal> values, bool hideLegend = false)
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
                IsVisibleAtLegend = !hideLegend,
                Fill = color
            };
        }

        public int SelectMonth
        {
            get
            {
                return selectMonth!.Value;
            }
            set
            {
                var newArray = new decimal[months.Count];
                selectMonth = value;
                newArray[selectMonth.Value] = (decimal)YAxis[0].MaxLimit;
                selected.SyncCollections(newArray);
            }
        }

        public void Update(OmrezninaReport mainReport)
        {
            var maxY = (double)mainReport.MonthlyReports.Max(x => Math.Max(x.EnergyPrice + x.OldFixedPrice + x.OldEnergyTransferPrice, x.EnergyPrice + x.FixedPowerPriceIfNo15Minute + x.AgreedPowerPrice + x.EnergyTransferPrice + x.OverdraftPowerPrice));
            var roundedMaxY = Math.Floor(maxY / 50.0) * 50.0 + 65.0;
            if (YAxis[0].MaxLimit != roundedMaxY)
            {
                YAxis[0].MaxLimit = roundedMaxY;
            }
            months.SyncCollections(mainReport.MonthlyReports.Select(x => UIHelper.MonthConverter(x.Month.Month, true)).ToArray());
            if (!selectMonth.HasValue || selectMonth >= months.Count)
            {
                var indexOfFirstMonthWithOverdraft = Array.IndexOf(mainReport.MonthlyReports, mainReport.MonthlyReports.FirstOrDefault(m => m.OverdraftPowerPrice > 0));
                if (indexOfFirstMonthWithOverdraft == -1)
                    indexOfFirstMonthWithOverdraft = 0;
                SelectMonth = indexOfFirstMonthWithOverdraft;
            }
            else
            {
                SelectMonth = SelectMonth;
            }
            oldFixedPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.OldFixedPrice).ToArray());
            oldEnergyTransportPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.OldEnergyTransferPrice).ToArray());
            if(!mainReport.CalculationOptions.Has15MinuteData)
            {
                agreedPowerPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.FixedPowerPriceIfNo15Minute).ToArray());
            }
            else
            {
                agreedPowerPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.AgreedPowerPrice).ToArray());
            }
            energyTransferPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.EnergyTransferPrice).ToArray());
            energyPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.EnergyPrice).ToArray());
            if (mainReport.CalculationOptions.IncludeEnergyPrice)
            {
                Series[3].IsVisibleAtLegend = true;
            }
            else
            {
                Series[3].IsVisibleAtLegend = false;
            }
            overdraftPrice.SyncCollections(mainReport.MonthlyReports.Select(m => m.OverdraftPowerPrice).ToArray());
            zeros.SyncCollections(new decimal[mainReport.MonthlyReports.Length]);
        }
    }
}
