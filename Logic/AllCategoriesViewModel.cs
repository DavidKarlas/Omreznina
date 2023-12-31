using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Omreznina.Client.Logic;
using Omreznina.Client.Pages;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Omreznina.Logic
{
    public class AllCategoriesViewModel
    {
        private readonly ObservableCollection<decimal> oldFixed = new([0]);
        private readonly ObservableCollection<decimal> oldEnergy = new([0]);

        private readonly ObservableCollection<decimal> agreedPowerPriceBlok1 = new([0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok2 = new([0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok3 = new([0]);
        private readonly ObservableCollection<decimal> agreedPowerPriceBlok4 = new([0]);
        private readonly ObservableCollection<decimal> energyPrice = new([0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok1 = new([0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok2 = new([0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok3 = new([0]);
        private readonly ObservableCollection<decimal> overdraftPriceBlok4 = new([0]);
        private readonly ObservableCollection<decimal> zeros = new([0]);
        public ISeries[] Series { get; private set; } = [];
        public Axis[] XAxis { get; private set; } = [];
        public Axis[] YAxis { get; private set; } = [];

        public AllCategoriesViewModel()
        {
            XAxis = [new Axis {
                Labels = []
            }];
            YAxis = [new Axis {
                MinLimit = 0,
                Labeler = value => ((decimal)value).ToEuroPreferFullNumber()
            }];
            Series = [
                    CreateStackedColumn("Star obračun: Moč", 0, UIHelper.OldFixedColor, oldFixed),
                    CreateStackedColumn("Star obračun: Energija", 0, UIHelper.OldEnergyColor, oldEnergy),
                    new StackedColumnSeries<decimal>
                    {
                        IsVisibleAtLegend = false,
                        Name="Star obračun: Suma",
                        Values = zeros,
                        Stroke = null,
                        DataLabelsSize = 12,
                        StackGroup = 0,
                        DataLabelsFormatter=point => ((decimal)point.StackedValue.Total).ToEuro(),
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsPosition = DataLabelsPosition.Top,
                        YToolTipLabelFormatter = point => ((decimal)point.StackedValue.Total).ToEuro()
                    },
                    CreateStackedColumn("Blok 1", 1, UIHelper.AgreedPowerBlockColors[0], agreedPowerPriceBlok1),
                    CreateStackedColumn("Blok 2", 1,UIHelper.AgreedPowerBlockColors[1], agreedPowerPriceBlok2),
                    CreateStackedColumn("Blok 3", 1,UIHelper.AgreedPowerBlockColors[2], agreedPowerPriceBlok3),
                    CreateStackedColumn("Blok 4", 1,UIHelper.AgreedPowerBlockColors[3], agreedPowerPriceBlok4),
                    CreateStackedColumn("Energija", 1,UIHelper.EnergyTransferColor, energyPrice),
                    CreateStackedColumn("Blok 1 - Prek", 1,UIHelper.OverdraftBlockColors[0], overdraftPriceBlok1),
                    CreateStackedColumn("Blok 2 - Prek", 1,UIHelper.OverdraftBlockColors[1], overdraftPriceBlok2),
                    CreateStackedColumn("Blok 3 - Prek", 1,UIHelper.OverdraftBlockColors[2], overdraftPriceBlok3),
                    CreateStackedColumn("Blok 4 - Prek", 1,UIHelper.OverdraftBlockColors[3], overdraftPriceBlok4),
                    new StackedColumnSeries<decimal>
                    {
                        IsVisibleAtLegend = false,
                        Name="Suma",
                        Values = zeros,
                        Stroke = null,
                        DataLabelsSize = 12,
                        StackGroup = 1,
                        DataLabelsFormatter=point => ((decimal)point.StackedValue.Total).ToEuro(),
                        DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        DataLabelsPosition = DataLabelsPosition.Top,
                        YToolTipLabelFormatter = point => ((decimal)point.StackedValue.Total).ToEuro()
                    }
                ];
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

        public void Update(OmrezninaReport mainReport)
        {
            if (mainReport.EnergyPrice != energyPrice[0])
                energyPrice[0] = mainReport.EnergyPrice;
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
            if (mainReport.OldFixedPrice != oldFixed[0])
                oldFixed[0] = mainReport.OldFixedPrice;
            if (mainReport.OldEnergyPrice != oldEnergy[0])
                oldEnergy[0] = mainReport.OldEnergyPrice;
        }
    }
}
