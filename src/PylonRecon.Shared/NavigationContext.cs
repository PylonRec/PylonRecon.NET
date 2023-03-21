using CommunityToolkit.Mvvm.ComponentModel;
using PylonRecon.Shared.Primitives;
using System;

namespace PylonRecon.Shared;

public class NavigationContext : ObservableObject
{
    private static NavigationContext? _instance;
    public static NavigationContext Instance => _instance ??= new();

    public event EventHandler<NavigationEventArgs>? NavigationCompleted;

    public void RaiseNavigationCompletedEvent(Type viewType)
    {
        NavigationCompleted?.Invoke(this, new NavigationEventArgs(viewType));
    }
}
