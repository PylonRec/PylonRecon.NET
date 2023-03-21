using HelixToolkit.Wpf;
using PylonRecon.IO;
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
            Shared.BrowseFileContext.Instance.FileChosen += Instance_FileChosen;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (Windows.UI.Xaml.Application.Current as UI.App)!.WindowHandle =
                new System.Windows.Interop.WindowInteropHelper(this).Handle;
        }

        private void Instance_FileChosen(object? sender, System.EventArgs e)
        {
            var path = Shared.BrowseFileContext.Instance.FilePath;
            var model = new PlyDocumentReader().ReadFrom(path);
            HelixViewport.Children.Clear();
            Point3DCollection points = new();
            model.ToList().ForEach(point => points.Add(new Point3D(point.Location.X, point.Location.Y, point.Location.Z)));
            HelixViewport.Children.Add(new PointsVisual3D
            {
                Points = points,
                Color = Colors.Red,
                Size = 5.0
            });
        }

        private void FrameHost_ChildChanged(object sender, System.EventArgs e)
        {
            if (FrameHost.GetUwpInternalObject() is Windows.UI.Xaml.Controls.Frame frame)
            {
                frame.Navigate(typeof(UI.Views.RootView));
            }
        }
    }
}
