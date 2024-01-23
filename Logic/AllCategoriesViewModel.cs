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
        private readonly ObservableCollection<decimal> oldFixed = new([0, 0]);
        private readonly ObservableCollection<decimal> oldEnergy = new([0, 0]);

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
                Text = "Skupna primerjava omrežnine",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public AllCategoriesViewModel()
        {
            YAxis = [new Axis {
                Labels = ["Nov obračun", "Star obračun"],
                Padding= new LiveChartsCore.Drawing.Padding(0,-15,5,-15),
                ShowSeparatorLines = false,
                TextSize = 13,
            }];
            XAxis = [new Axis {
                MinLimit = 0,
                LabelsPaint=null
            }];
            Series = [
                    CreateStackedColumn("Star obračun: Omrežnina Moč", 1, UIHelper.AgreedPowerColor, oldFixed),
                    CreateStackedColumn("Star obračun: Omrežnina Poraba", 1, UIHelper.EnergyTransferColor, oldEnergy),
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

        public void Update(OmrezninaReport mainReport)
        {
            if (mainReport.CalculationOptions.IncludeEnergyPrice)
            {
                Series[8].IsVisibleAtLegend = true;
            }
            else
            {
                Series[8].IsVisibleAtLegend = false;
            }

            if (!mainReport.CalculationOptions.Has15MinuteData)
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

            if (mainReport.FixedPowerPriceIfNo15Minute != fixedPowerPriceNo15MinMesures[0])
                fixedPowerPriceNo15MinMesures[0] = mainReport.FixedPowerPriceIfNo15Minute;

            if (mainReport.EnergyTransferPrice != energyTransferPrice[0])
                energyTransferPrice[0] = mainReport.EnergyTransferPrice;

            if (mainReport.EnergyPrice != energyPrice[0])
                energyPrice[0] = mainReport.EnergyPrice;
            if (mainReport.EnergyPrice != energyPrice[1])
                energyPrice[1] = mainReport.EnergyPrice;

            if (mainReport.AgreedPowerPricePerBlock[0] != agreedPowerPriceBlok1[0])
                agreedPowerPriceBlok1[0] = mainReport.AgreedPowerPricePerBlock[0];
            if (mainReport.AgreedPowerPricePerBlock[1] != agreedPowerPriceBlok2[0])
                agreedPowerPriceBlok2[0] = mainReport.AgreedPowerPricePerBlock[1];
            if (mainReport.AgreedPowerPricePerBlock[2] != agreedPowerPriceBlok3[0])
                agreedPowerPriceBlok3[0] = mainReport.AgreedPowerPricePerBlock[2];
            if (mainReport.AgreedPowerPricePerBlock[3] != agreedPowerPriceBlok4[0])
                agreedPowerPriceBlok4[0] = mainReport.AgreedPowerPricePerBlock[3];

            if (mainReport.OverdraftPowerPricePerBlock[0] != overdraftPriceBlok1[0])
                overdraftPriceBlok1[0] = mainReport.OverdraftPowerPricePerBlock[0];
            if (mainReport.OverdraftPowerPricePerBlock[1] != overdraftPriceBlok2[0])
                overdraftPriceBlok2[0] = mainReport.OverdraftPowerPricePerBlock[1];
            if (mainReport.OverdraftPowerPricePerBlock[2] != overdraftPriceBlok3[0])
                overdraftPriceBlok3[0] = mainReport.OverdraftPowerPricePerBlock[2];
            if (mainReport.OverdraftPowerPricePerBlock[3] != overdraftPriceBlok4[0])
                overdraftPriceBlok4[0] = mainReport.OverdraftPowerPricePerBlock[3];

            if (mainReport.OldFixedPrice != oldFixed[1])
                oldFixed[1] = mainReport.OldFixedPrice;
            if (mainReport.OldEnergyTransferPrice != oldEnergy[1])
                oldEnergy[1] = mainReport.OldEnergyTransferPrice;

            var maxValue = (double)Math.Max(
                mainReport.OldFixedPrice + 
                mainReport.OldEnergyTransferPrice +
                mainReport.EnergyPrice,

                mainReport.EnergyTransferPrice + 
                mainReport.OverdraftPowerPrice +
                mainReport.AgreedPowerPrice +
                mainReport.FixedPowerPriceIfNo15Minute +
                mainReport.EnergyPrice);
            if (maxValue != XAxis[0].MaxLimit)
                XAxis[0].MaxLimit = maxValue * 1.05;
        }
    }
}
