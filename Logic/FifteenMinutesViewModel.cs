using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Omreznina.Client.Logic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using Omreznina.Client.Pages;
using LiveChartsCore.Measure;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace Omreznina.Logic
{
    public class FifteenMinutesViewModel
    {
        private readonly ObservableCollection<string> timesOfDay = new();
        private readonly ObservableCollection<decimal> overdraftPrice = new();
        private readonly ObservableCollection<decimal> agreedPower = new();
        private readonly ObservableCollection<decimal> actualPower = new();
        private readonly ObservableCollection<decimal> returnPower = new();

        public ISeries[] Series { get; }
        public Axis[] XAxis { get; }
        public Axis[] YAxis { get; }
        public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

        public FifteenMinutesViewModel()
        {
            Series = [
                new LineSeries<decimal>{
                     Values = actualPower,
                     Stroke="#00A9FF".ToPaint(4),
                     Fill=null,
                     GeometrySize=0,
                     GeometryStroke = null,
                     GeometryFill = null,
                     ScalesYAt=1,
                     Name = "Dejanska moč"
                },
                new LineSeries<decimal>{
                     Values = returnPower,
                     Stroke="#00e600".ToPaint(4),
                     Fill=null,
                     GeometrySize=0,
                     GeometryStroke = null,
                     GeometryFill = null,
                     ScalesYAt=1,
                     Name = "Višek samoproizvodnje"
                },
               new ColumnSeries<decimal>
               {
                    Values = overdraftPrice,
                    Stroke = null,
                    Fill = UIHelper.OverdraftColor,
                    ScalesYAt=0,
                    DataLabelsPaint="#151515".ToPaint(),
                    DataLabelsPosition = DataLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.Model==0 ? "": point.Model.ToEuro(),
                    Name = "Prekoračitev",
                    DataLabelsSize=12,
                    DataLabelsRotation=90,
                    DataLabelsPadding= new LiveChartsCore.Drawing.Padding(0,0,0,0)
                }
                ];
            XAxis = [new Axis {
                Labels = timesOfDay,
                MinLimit=0,
                MaxLimit=95,
                Padding= new LiveChartsCore.Drawing.Padding(-15,0,-15,0)
            }];
            YAxis = [new Axis {
                MinLimit=0,
                ShowSeparatorLines=false,
                Position= AxisPosition.End,
                Labeler = value => ((decimal)value).ToEuroPreferFullNumber()
            },
            new Axis{
                MinLimit=0,
                Labeler= value => ((decimal)value).ToKW()
            }];
        }

        public void Update(CalculationOptions options, DayReport dayReport)
        {
            timesOfDay.SyncCollections(dayReport.Usages.Select(u => u.Source.DateTime.Minute == 0 ? u.Source.DateTime.ToString("HH:mm") : "").ToArray());
            overdraftPrice.SyncCollections(dayReport.Usages.Select(u => u.OverdraftPrice).ToArray());
            actualPower.SyncCollections(dayReport.Usages.Select(u => u.Source.ConsumedPower).ToArray());
            returnPower.SyncCollections(dayReport.Usages.Select(u => u.Source.GivenPower).ToArray());
            var maxPower = dayReport.Usages.Max(u => Math.Max(u.Source.ConsumedPower, u.Source.GivenPower));
            var visuals = new List<GeometryVisual<RectangleGeometry>>();
            var lowBlock = dayReport.Usages[0].Source.Block;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[lowBlock]);
            var mediumBlock = dayReport.Usages[6 * 4].Source.Block;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[mediumBlock]);
            var highBlock = dayReport.Usages[7 * 4].Source.Block;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[highBlock]);
            YAxis[1].MaxLimit = Math.Round((double)maxPower + 2);
            VisualElements = [
                CreateBlockVisual(lowBlock, options, 0, 6),
                CreateBlockVisual(mediumBlock, options, 6, 7),
                CreateBlockVisual(highBlock, options, 7, 14),
                CreateBlockVisual(mediumBlock, options, 14, 16),
                CreateBlockVisual(highBlock, options, 16, 20),
                CreateBlockVisual(mediumBlock, options, 20, 22),
                CreateBlockVisual(lowBlock, options, 22, 24)
                ];
        }

        private static GeometryVisual<RectangleGeometry> CreateBlockVisual(int blockIndex, CalculationOptions options, int startHour, int endHour)
        {
            var blockPowerKW = (double)options.AgreedMaxPowerBlocks[blockIndex];
            return new GeometryVisual<RectangleGeometry> {
                ScalesYAt = 1,
                X = startHour * 4,
                Y = blockPowerKW,
                LocationUnit = MeasureUnit.ChartValues,
                Width = (endHour - startHour) * 4 - (endHour == 24 ? 1 : 0),
                Height = blockPowerKW,
                SizeUnit = MeasureUnit.ChartValues,
                ScalesXAt = 0,
                Fill = new SolidColorPaint(UIHelper.AgreedPowerBlockColors[blockIndex].Color) {
                    ZIndex = -10
                },
                Stroke = null,
                Label = $"Blok {blockIndex + 1}",
                LabelPaint = new SolidColorPaint(new SKColor(220, 220, 220)) { ZIndex = -1 },
                LabelSize = 14
            };
        }
    }
}
