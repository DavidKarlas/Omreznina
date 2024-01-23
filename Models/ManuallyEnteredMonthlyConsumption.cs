using CommunityToolkit.Mvvm.ComponentModel;

namespace Omreznina.Models
{
    public partial class ManuallyEnteredMonthlyConsumption : ObservableObject
    {
        [ObservableProperty]
        private decimal singleTarif;
        [ObservableProperty]
        private decimal lowTarif;
        [ObservableProperty]
        private decimal highTarif;
    }
}
