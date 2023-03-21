using PylonRecon.UI;
using System;

namespace PylonRecon.Wpf;

public class Program
{
    [STAThread]
    public static void Main()
    {
        using var uiApp = new UI.App();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
