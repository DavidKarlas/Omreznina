using LiveChartsCore.SkiaSharpView.SKCharts;
using Omreznina.Logic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Omreznina
{
    public class PdfExporter
    {
        public static MemoryStream Export(ViewModel viewModel)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var ms = new MemoryStream();
            Document.Create(container => {
                _ = container.Page(page => {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Mesecni pregled")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(0, Unit.Centimetre)
                        .Column(x => {
                            x.Item()
                                .Canvas((canvas, size) => {
                                    var cartesianChart = new SKCartesianChart {
                                        Width = (int)size.Width,
                                        Height = (int)size.Height,
                                        Series = viewModel.MonthsViewModel.Series,
                                        XAxes = viewModel.MonthsViewModel.XAxis,
                                        YAxes = viewModel.MonthsViewModel.YAxis
                                    };

                                    cartesianChart.SaveImage(canvas);
                                });
                        });
                });
            })
.GeneratePdf(ms);
            ms.Position = 0;
            return ms;
        }
    }
}
