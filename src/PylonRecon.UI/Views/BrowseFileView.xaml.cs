using CommunityToolkit.Mvvm.Input;
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml.Controls;

namespace PylonRecon.UI.Views
{
    public sealed partial class BrowseFileView : Page
    {
        public BrowseFileView()
        {
            this.InitializeComponent();
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
                FileTypeFilter = { ".ply" }
            };
            IntPtr hwnd = (Windows.UI.Xaml.Application.Current as App).WindowHandle;
            ((IInitializeWithWindow)(object)picker).Initialize(hwnd);
            if (await picker.PickSingleFileAsync() is not { } file) return;
            Shared.BrowseFileContext.Instance.FilePath = file.Path;
        }
    }
}
