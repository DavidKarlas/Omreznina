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
        private readonly ObservableCollection<CalculatedUsage> usages = new();

        public ISeries[] Series { get; }
        public Axis[] XAxis { get; }
        public Axis[] YAxis { get; }
        public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

        public LabelVisual Title { get; set; } =
            new LabelVisual {
                Text = "Izberi dan",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public FifteenMinutesViewModel()
        {
            Series = [
                new LineSeries<CalculatedUsage>{
                     XToolTipLabelFormatter = value => FormatTime(value.Coordinate.SecondaryValue).ToString("HH:mm"),
                    Mapping = (usage, index) => new Coordinate(index, (double)usage.Source.ImportPower),
                    Values = usages,
                     Stroke="#00A9FF".ToPaint(4),
                     Fill=null,
                     GeometrySize=0,
                     GeometryStroke = null,
                     GeometryFill = null,
                     ScalesYAt=1,
                     Name = "Dejanska moč"
                },
                new LineSeries<CalculatedUsage>{
                     Values = usages,
                     Mapping = (usage, index) => new Coordinate(index, (double)usage.Source.ExportPower),
                     Stroke="#00e600".ToPaint(4),
                     Fill=null,
                     GeometrySize=0,
                     GeometryStroke = null,
                     GeometryFill = null,
                     ScalesYAt=1,
                     Name = "Višek samoproizvodnje"
                },
               new ColumnSeries<CalculatedUsage>
               {
                    Values = usages,
                    Mapping = (usage, index) => new Coordinate(index, (double)usage.OverdraftPrice),
                    Stroke = null,
                    Fill = UIHelper.OverdraftColor,
                    ScalesYAt=0,
                    DataLabelsPaint="#151515".ToPaint(),
                    DataLabelsPosition = DataLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue==0 ? "": point.PrimaryValue.ToEuro(),
                    Name = "Prekoračitev",
                    DataLabelsSize=12,
                    DataLabelsRotation=90,
                    DataLabelsPadding= new LiveChartsCore.Drawing.Padding(0,0,0,0)
                }
                ];
            XAxis = [new Axis {
                Labeler = value => FormatTime(value).ToStringFullHours(),
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

        TimeOnly FormatTime(double index)
        {
            var extraHour = this.extraHour * -4;
            if (index < 3 * 4)
                extraHour = 0;
            return TimeOnly.FromTimeSpan(TimeSpan.FromMinutes((index + extraHour) * 15));
        }

        public bool IsVisible { get; set; } = false;
        int extraHour = 0;
        public void Update(CalculationOptions options, DayReport? dayReport)
        {
            if (dayReport == null)
            {
                IsVisible = false;
                return;
            }
            IsVisible = true;
            Title.Text = $"15 minutni odčitki za {dayReport.Day:dd.MM.yyyy}";
            usages.SyncCollections(dayReport.Usages);
            var maxPower = dayReport.Usages.Max(u => Math.Max(u.Source.ImportPower, u.Source.ExportPower));
            var visuals = new List<GeometryVisual<RectangleGeometry>>();
            var lowBlock = dayReport.Usages[0].Source.Block;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[lowBlock]);
            var mediumBlock = lowBlock - 1;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[mediumBlock]);
            var highBlock = mediumBlock - 1;
            maxPower = Math.Max(maxPower, options.AgreedMaxPowerBlocks[highBlock]);
            YAxis[1].MaxLimit = Math.Round((double)maxPower + 2);
            if (dayReport.Usages.Count == 4 * 25)
            {
                extraHour = 1;
                XAxis[0].MaxLimit = 25 * 4;
            }
            else if (dayReport.Usages.Count == 4 * 23)
            {
                extraHour = -1;
                XAxis[0].MaxLimit = 23 * 4;
            }
            else
            {
                extraHour = 0;
                XAxis[0].MaxLimit = 24 * 4;
            }
            VisualElements = [
                CreateBlockVisual(lowBlock, options, 0, 6 + extraHour),
                CreateBlockVisual(mediumBlock, options, 6 + extraHour, 7 + extraHour),
                CreateBlockVisual(highBlock, options, 7 + extraHour, 14 + extraHour),
                CreateBlockVisual(mediumBlock, options, 14 + extraHour, 16 + extraHour),
                CreateBlockVisual(highBlock, options, 16 + extraHour, 20 + extraHour),
                CreateBlockVisual(mediumBlock, options, 20 + extraHour, 22 + extraHour),
                CreateBlockVisual(lowBlock, options, 22 + extraHour, 24 + extraHour)
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
                Width = (endHour - startHour) * 4 - (endHour >= 24 ? 1 : 0),
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
