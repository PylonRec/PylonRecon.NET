using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PylonRecon.Shared;

public partial class BrowseFileContext : ObservableObject
{
    private static BrowseFileContext? _instance;
    public static BrowseFileContext Instance => _instance ??= new();

    [ObservableProperty]
    private string _filePath;

    public event EventHandler<EventArgs>? FileChosen;

    partial void OnFilePathChanged(string value)
    {
        FileChosen?.Invoke(this, EventArgs.Empty);
    }
}
