using CommunityToolkit.Mvvm.Input;
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using PylonRecon.Shared;

namespace PylonRecon.UI.Views
{
    [ObservableObject]
    public sealed partial class BrowseFileView : Page
    {
        [ObservableProperty] private string _pointCloudProperties = string.Empty;
        [ObservableProperty] private bool _pointCloudLoaded = false;
        
        
        public BrowseFileView()
        {
            this.InitializeComponent();
            BindBridge();
        }

        private void BindBridge()
        {
            BrowseFileBridge.Instance.PointCloudLoadedAction = OnPointCloudLoaded;
        }

        private void OnPointCloudLoaded(PointCloud cloud)
        {
            PointCloudProperties = $"点云规模: {cloud.Count}\n" +
                                   $"X: [{cloud.XLimits.Item1}, {cloud.XLimits.Item2}]\n" +
                                   $"Y: [{cloud.YLimits.Item1}, {cloud.YLimits.Item2}]\n" +
                                   $"Z: [{cloud.ZLimits.Item1}, {cloud.ZLimits.Item2}]";
            PointCloudLoaded = true;
        }

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

        [RelayCommand]
        private async void BrowseFile()
        {
            FileOpenPicker picker = new()
            {
                FileTypeFilter = { ".ply", ".xyz" }
            };
            IntPtr hwnd = (Windows.UI.Xaml.Application.Current as App).WindowHandle;
            ((IInitializeWithWindow)(object)picker).Initialize(hwnd);
            if (await picker.PickSingleFileAsync() is not { } file) return;
            Shared.BrowseFileBridge.Instance.OpenFile(file.Path);
        }

        [RelayCommand]
        private void NextView()
        {
            Frame.Navigate(typeof(AxisFixView));
        }
    }
}
