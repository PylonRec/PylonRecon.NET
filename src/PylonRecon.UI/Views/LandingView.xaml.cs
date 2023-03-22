using CommunityToolkit.Mvvm.Input;
using Windows.UI.Xaml.Controls;

namespace PylonRecon.UI.Views
{
    public sealed partial class LandingView : Page
    {
        public LandingView()
        {
            this.InitializeComponent();
        }

        [RelayCommand]
        private void Start()
        {
            Frame.Navigate(typeof(BrowseFileView));
        }
    }
}
