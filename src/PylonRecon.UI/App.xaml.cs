using Microsoft.Toolkit.Win32.UI.XamlHost;
using System;

namespace PylonRecon.UI
{
    public sealed partial class App : XamlApplication
    {
        public IntPtr WindowHandle { get; set; }

        public App()
        {
            this.Initialize();
        }
    }
}
