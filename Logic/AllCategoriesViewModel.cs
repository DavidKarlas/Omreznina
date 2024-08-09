using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Omreznina.Client.Logic;
using Omreznina.Client.Pages;
using SkiaSharp;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace Omreznina.Logic
{
    public class AllCategoriesViewModel
    {
        private readonly ObservableCollection<decimal> fixedPowerPriceNo15MinMesures = new([0, 0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok1 = new([0, 0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok2 = new([0, 0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok3 = new([0, 0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok4 = new([0, 0]);
        private readonly ObservableCollection<decimal> energyTransferPrice = new([0, 0]);
        private readonly ObservableCollection<decimal> energyPrice = new([0, 0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok1 = new([0, 0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok2 = new([0, 0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok3 = new([0, 0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok4 = new([0, 0]);
        private readonly ObservableCollection<decimal> zeros = new([0, 0]);

        public ISeries[] Series { get; private set; } = [];
        public Axis[] XAxis { get; private set; } = [];
        public Axis[] YAxis { get; private set; } = [];

        public LabelVisual Title { get; set; } =
            new LabelVisual {
                Text = "Skupna primerjava stroškov",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public AllCategoriesViewModel()
        {
            YAxis = [new Axis {
                Labels = ["Novi stroški", "Trenutni stroški"],
                Padding= new LiveChartsCore.Drawing.Padding(0, -15, 5, -15),
                ShowSeparatorLines = false,
                TextSize = 13,
            }];
            XAxis = [new Axis {
                MinLimit = 0,
                LabelsPaint=null
            }];
            Series = [
                    CreateStackedColumn("Omrežnina Moč", 1, UIHelper.AgreedPowerColor, fixedPowerPriceNo15MinMesures),
                    CreateStackedColumn("Blok 1", 1, UIHelper.AgreedPowerBlockColors[0], agreedPowerPriceBlok1),
                    CreateStackedColumn("Blok 2", 1, UIHelper.AgreedPowerBlockColors[1], agreedPowerPriceBlok2),
                    CreateStackedColumn("Blok 3", 1, UIHelper.AgreedPowerBlockColors[2], agreedPowerPriceBlok3),
                    CreateStackedColumn("Blok 4", 1, UIHelper.AgreedPowerBlockColors[3], agreedPowerPriceBlok4),
                    CreateStackedColumn("Omrežnina Poraba", 1, UIHelper.EnergyTransferColor, energyTransferPrice),
                    CreateStackedColumn("Energija", 1, UIHelper.EnergyColor, energyPrice),
                    CreateStackedColumn("Blok 1 - Preko.", 1, UIHelper.OverdraftBlockColors[0], overdraftPriceBlok1),
                    CreateStackedColumn("Blok 2 - Preko.", 1, UIHelper.OverdraftBlockColors[1], overdraftPriceBlok2),
                    CreateStackedColumn("Blok 3 - Preko.", 1, UIHelper.OverdraftBlockColors[2], overdraftPriceBlok3),
                    CreateStackedColumn("Blok 4 - Preko.", 1, UIHelper.OverdraftBlockColors[3], overdraftPriceBlok4),
                    new StackedRowSeries<decimal>
                    {
                        IsVisibleAtLegend = false,
                        Name="Suma",
                        Values = zeros,
                        Padding = 1,
                        DataPadding = new LiveChartsCore.Drawing.LvcPoint(1,1),
                        Stroke = null,
                        DataLabelsSize = 12,
                        StackGroup = 1,
                        DataLabelsFormatter=point => ((decimal)point.StackedValue.Total).ToEuro(),
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsPosition = DataLabelsPosition.Right,
                        YToolTipLabelFormatter = point => ((decimal)point.StackedValue.Total).ToEuro()
                    }
                ];
        }

        private StackedRowSeries<decimal> CreateStackedColumn(string name, int stackGroup, SolidColorPaint color, IEnumerable<decimal> values, bool hideLegend = false)
        {
            return new StackedRowSeries<decimal> {
                Name = name,
                Values = values,
                Stroke = null,
                StackGroup = stackGroup,
                Padding = 1,
                DataPadding = new LiveChartsCore.Drawing.LvcPoint(1, 1),
                DataLabelsSize = 11,
                DataLabelsFormatter = point => {
                    if (XAxis[0].VisibleDataBounds.Max == 0)
                        return "";
                    if (((double)point.Model / XAxis[0].VisibleDataBounds.Max) < 0.02)
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

        public void Update(OmrezninaReport leftReport, OmrezninaReport rightReport)
        {
            if (!leftReport.CalculationOptions.Has15MinuteData)
            {
                Series[2].IsVisibleAtLegend = true;

                Series[3].IsVisibleAtLegend = false;
                Series[4].IsVisibleAtLegend = false;
                Series[5].IsVisibleAtLegend = false;
                Series[6].IsVisibleAtLegend = false;
                Series[^5].IsVisibleAtLegend = false;
                Series[^4].IsVisibleAtLegend = false;
                Series[^3].IsVisibleAtLegend = false;
                Series[^2].IsVisibleAtLegend = false;
            }
            else
            {
                Series[2].IsVisibleAtLegend = false;

                Series[3].IsVisibleAtLegend = true;
                Series[4].IsVisibleAtLegend = true;
                Series[5].IsVisibleAtLegend = true;
                Series[6].IsVisibleAtLegend = true;
                Series[^5].IsVisibleAtLegend = true;
                Series[^4].IsVisibleAtLegend = true;
                Series[^3].IsVisibleAtLegend = true;
                Series[^2].IsVisibleAtLegend = true;
            }

            UpdateData(leftReport, 1);
            UpdateData(rightReport, 0);

            var maxValue = (double)Math.Max(
                leftReport.TotalPrice,
                rightReport.TotalPrice);
            if (maxValue != XAxis[0].MaxLimit)
                XAxis[0].MaxLimit = maxValue * 1.05;
        }

        private void UpdateData(OmrezninaReport report, int index)
        {
            if (report.FixedPowerPriceIfNo15Minute != fixedPowerPriceNo15MinMesures[index])
                fixedPowerPriceNo15MinMesures[index] = report.FixedPowerPriceIfNo15Minute;

            if (report.EnergyTransferPrice != energyTransferPrice[index])
                energyTransferPrice[index] = report.EnergyTransferPrice;

            if (report.EnergyPrice != energyPrice[index])
                energyPrice[index] = report.EnergyPrice;
            if (report.EnergyPrice != energyPrice[1])
                energyPrice[1] = report.EnergyPrice;

            if (report.AgreedPowerPricePerBlock[index] != agreedPowerPriceBlok1[index])
                agreedPowerPriceBlok1[index] = report.AgreedPowerPricePerBlock[index];
            if (report.AgreedPowerPricePerBlock[1] != agreedPowerPriceBlok2[index])
                agreedPowerPriceBlok2[index] = report.AgreedPowerPricePerBlock[1];
            if (report.AgreedPowerPricePerBlock[2] != agreedPowerPriceBlok3[index])
                agreedPowerPriceBlok3[index] = report.AgreedPowerPricePerBlock[2];
            if (report.AgreedPowerPricePerBlock[3] != agreedPowerPriceBlok4[index])
                agreedPowerPriceBlok4[index] = report.AgreedPowerPricePerBlock[3];

            if (report.OverdraftPowerPricePerBlock[index] != overdraftPriceBlok1[index])
                overdraftPriceBlok1[index] = report.OverdraftPowerPricePerBlock[index];
            if (report.OverdraftPowerPricePerBlock[1] != overdraftPriceBlok2[index])
                overdraftPriceBlok2[index] = report.OverdraftPowerPricePerBlock[1];
            if (report.OverdraftPowerPricePerBlock[2] != overdraftPriceBlok3[index])
                overdraftPriceBlok3[index] = report.OverdraftPowerPricePerBlock[2];
            if (report.OverdraftPowerPricePerBlock[3] != overdraftPriceBlok4[index])
                overdraftPriceBlok4[index] = report.OverdraftPowerPricePerBlock[3];
        }
    }
}
