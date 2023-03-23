using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PylonRecon.UI.Views
{
    [ObservableObject]
    public sealed partial class SegmentationView : Page
    {
        [ObservableProperty] private double _layerCloudDensity = 50;
        
        public SegmentationView()
        {
            this.InitializeComponent();
        }

        [RelayCommand]
        private void NextView()
        {
            Frame.Navigate(typeof(SegmentExtractionView));
        }
    }
}
