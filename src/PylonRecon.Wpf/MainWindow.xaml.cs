using HelixToolkit.Wpf;
using PylonRecon.IO;
using PylonRecon.Wpf.BridgeOperations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Windows.UI.Xaml.Controls;

namespace PylonRecon.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (Windows.UI.Xaml.Application.Current as UI.App)!.WindowHandle =
                new System.Windows.Interop.WindowInteropHelper(this).Handle;
            BindContextActions();
        }

        private void BindContextActions()
        {
            new BrowseFileActions(HelixViewport).Bind();
        }

        private void FrameHost_ChildChanged(object sender, System.EventArgs e)
        {
            if (FrameHost.GetUwpInternalObject() is Windows.UI.Xaml.Controls.Frame frame)
            {
                frame.Navigate(typeof(UI.Views.LandingView));
            }
        }
    }
}
