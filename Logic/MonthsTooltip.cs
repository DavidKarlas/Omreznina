using System;
using System.Collections.Generic;
using System.Drawing;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Omreznina.Client.Pages;
using SkiaSharp;

namespace Omreznina.Logic;

public class MonthsTooltip : IChartTooltip<SkiaSharpDrawingContext>
{
    private LabelVisual _monthNameTitleLabel;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;
    private TableLayout<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _mainTable;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _leftStackPanel;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _rightStackPanel;
    private static readonly int s_zIndex = 10100;
    private readonly SolidColorPaint _backgroundPaint = new(new SKColor(230, 230, 230)) { ZIndex = s_zIndex };
    private readonly SolidColorPaint _fontPaint = new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex + 1 };

    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        if (_stackPanel is null)
        {
            _stackPanel = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> {
                Padding = new Padding(10),
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Middle,
                VerticalAlignment = Align.Middle,
                BackgroundPaint = _backgroundPaint
            };
            _monthNameTitleLabel = new LabelVisual() {
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(15, 0, 0, 0),
                ClippingMode = ClipMode.None, // required on tooltips // mark
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle
            };
            _stackPanel.Children.Add(_monthNameTitleLabel);
            _mainTable = new TableLayout<RoundedRectangleGeometry, SkiaSharpDrawingContext>();
            _stackPanel.Children.Add(_mainTable);
            _leftStackPanel = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> {
                Padding = new Padding(10),
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle,
                BackgroundPaint = _backgroundPaint
            };
            var leftTitleLabel = new LabelVisual() {
                Text = "Star obračun:",
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(0, 0, 0, 0),
                ClippingMode = ClipMode.None, // required on tooltips // mark
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            };
            _mainTable.AddChild(leftTitleLabel, 0, 0);
            _mainTable.AddChild(_leftStackPanel, 1, 0);
            _rightStackPanel = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> {
                Padding = new Padding(10),
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle,
                BackgroundPaint = _backgroundPaint
            };
            var rightTitleLabel = new LabelVisual() {
                Text = "Nov obračun:",
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(0, 0, 0, 0),
                ClippingMode = ClipMode.None, // required on tooltips // mark
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            };
            _mainTable.AddChild(rightTitleLabel, 0, 1);
            _mainTable.AddChild(_rightStackPanel, 1, 1);
        }

        // clear the previous elements.
        foreach (var child in _rightStackPanel!.Children.ToArray())
        {
            _ = _rightStackPanel.Children.Remove(child);
            chart.RemoveVisual(child);
        }
        foreach (var child in _leftStackPanel!.Children.ToArray())
        {
            _ = _leftStackPanel.Children.Remove(child);
            chart.RemoveVisual(child);
        }



        foreach (var point in foundPoints)
        {
            _monthNameTitleLabel.Text = point.Context.Series.GetSecondaryToolTipText(point);

            var sketch = ((IChartSeries<SkiaSharpDrawingContext>)point.Context.Series).GetMiniaturesSketch();
            var relativePanel = sketch.AsDrawnControl(s_zIndex);

            var labelName = new LabelVisual {
                Text = $"{point.Context.Series.Name.Replace("Star obračun: ", "")}:",
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(4, 0, 0, 0),
                ClippingMode = ClipMode.None, // required on tooltips // mark
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            };

            var labelValue = new LabelVisual {
                Text = point.Context.Series.Name.Contains("Suma") ? point.AsDataLabel : ((decimal)point.Context.DataSource).ToEuro(),
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(4, 0, 0, 0),
                ClippingMode = ClipMode.None, // required on tooltips // mark
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            };

            var sp = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> {
                Padding = new Padding(0, 0),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    relativePanel,
                    labelName,
                    labelValue
                }
            };
            if (point.Context.Series is StackedColumnSeries<decimal> stacked && stacked.StackGroup == 0)
                _leftStackPanel.Children.Add(sp);
            else
                _rightStackPanel.Children.Add(sp);
        }

        var size = _stackPanel.Measure(chart);

        var location = foundPoints.GetTooltipLocation(size, chart);

        _stackPanel.X = location.X;
        _stackPanel.Y = location.Y;

        chart.AddVisual(_stackPanel);
    }

    public void Hide(Chart<SkiaSharpDrawingContext> chart)
    {
        if (chart is null || _stackPanel is null) return;
        chart.RemoveVisual(_stackPanel);
    }
}
