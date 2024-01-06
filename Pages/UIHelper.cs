using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Omreznina.Client.Pages
{
    public static class UIHelper
    {
        public static string MonthConverter(int month, bool shortVersion)
        {
            if (shortVersion)
            {
                switch (month)
                {
                    case 1: return "Jan";
                    case 2: return "Feb";
                    case 3: return "Mar";
                    case 4: return "Apr";
                    case 5: return "Maj";
                    case 6: return "Jun";
                    case 7: return "Jul";
                    case 8: return "Avg";
                    case 9: return "Sep";
                    case 10: return "Oct";
                    case 11: return "Nov";
                    case 12: return "Dec";
                }
            }
            else
            {

                switch (month)
                {
                    case 1: return "Januar";
                    case 2: return "Februar";
                    case 3: return "Marec";
                    case 4: return "April";
                    case 5: return "Maj";
                    case 6: return "Junij";
                    case 7: return "Julij";
                    case 8: return "Avgust";
                    case 9: return "September";
                    case 10: return "October";
                    case 11: return "November";
                    case 12: return "December";
                }
            }
            return "N/A";
        }
        public const string EuroConverter = "function(value, opts) { var num = Number(value); return (num%1==0?num.toFixed(0):num.toFixed(2)) + ' €';}";
        public const string KWConverter = "function(value, opts) { var num = Number(value); return (num%1==0?num.toFixed(0):num.toFixed(2)) + ' kW';}";
        public const string EuroConverterHideZero = "function(value, opts) { if (value == 0) return ''; var num = Number(value); return (num%1==0?num.toFixed(0):num.toFixed(2)) + ' €';}";

        public static string ToEuro(this decimal value, int decimals = 2) =>decimals==2? $"{value:0.00}€": $"{value:0.00000}€";
        public static string ToEuroPreferFullNumber(this decimal value) => value % 1 == 0 ? $"{value:0}€" : $"{value:0.00}€";
        public static string ToKW(this decimal value) => $"{value:0.00} kW";
        public static string ToKW(this double value) => $"{value:0.00} kW";

        public static SolidColorPaint OldFixedColor { get => "#CC8F00".ToPaint(); }
        public static SolidColorPaint OldEnergyColor { get => "#05a47b".ToPaint(); }


        public static SolidColorPaint EnergyTransferColor { get => "#05a47b".ToPaint(); }
        public static SolidColorPaint AgreedPowerColor { get => "#CC8F00".ToPaint(); }
        public static SolidColorPaint OverdraftColor { get => "#A90F33".ToPaint(); }
        public static readonly SolidColorPaint[] AgreedPowerBlockColors = [
            "#8F6400".ToPaint(),
            "#CC8F00".ToPaint(),
            "#F5AB00".ToPaint(),
            "#FFC233".ToPaint(),
            "#FFD470".ToPaint()
            ];
        public static readonly SolidColorPaint[] OverdraftBlockColors = [
            "#710A22".ToPaint(),
            "#A90F33".ToPaint(),
            "#E11444".ToPaint(),
            "#EF436B".ToPaint(),
            "#F47C98".ToPaint()
            ];
    }
}
