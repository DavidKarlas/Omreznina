using CommunityToolkit.Mvvm.ComponentModel;

namespace Omreznina.Logic
{
    public partial class GlobalCalculationOptions : ObservableObject
    {
        [ObservableProperty]
        private bool has15MinuteData;
    }
}
