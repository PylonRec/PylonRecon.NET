using System.Windows;
using Windows.UI.Xaml.Controls;

namespace PylonRecon.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FrameHost_ChildChanged(object sender, System.EventArgs e)
        {
            if (FrameHost.GetUwpInternalObject() is Frame frame)
            {
                frame.Navigate(typeof(UI.Views.RootView));
            }
        }
    }
}
