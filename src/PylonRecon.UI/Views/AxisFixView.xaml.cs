using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using PylonRecon.Geometry;
using PylonRecon.Shared;

namespace PylonRecon.UI.Views
{
    [ObservableObject]
    public sealed partial class AxisFixView : Page
    {
        public bool ManualAxisCaptured => ManualAxis is not null;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ManualAxisCaptured))]
        private Vector3D? _manualAxis;

        [ObservableProperty] private string _manualAxisInfo = string.Empty;

        [ObservableProperty] private int _initialPopulation = 30;
        [ObservableProperty] private double _crossoverProbability = 0.6d;
        [ObservableProperty] private double _mutationProbability = 0.1d;
        [ObservableProperty] private double _parentRemainingRatio = 0.4d;
        [ObservableProperty] private double _maxIteration = 10;
        
        public AxisFixView()
        {
            this.InitializeComponent();
        }

        [RelayCommand]
        private void CaptureManualAxis()
        {
            ManualAxis = AxisFixBridge.Instance.CaptureAxisDirection();
            if (ManualAxis is not null)
            {
                ManualAxisInfo = $"手动调整记录值\n" +
                                 $"X: {ManualAxis.X}\n" +
                                 $"Y: {ManualAxis.Y}\n" +
                                 $"Z: {ManualAxis.Z}";
            }
        }

        [RelayCommand]
        private void NextView()
        {
            Frame.Navigate(typeof(SegmentationView));
        }
    }
}
