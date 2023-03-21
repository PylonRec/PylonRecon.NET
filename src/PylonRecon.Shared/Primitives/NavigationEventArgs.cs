using System;

namespace PylonRecon.Shared.Primitives;

public class NavigationEventArgs : EventArgs
{
    public Type ViewType { get; set; }

    public NavigationEventArgs(Type viewType)
    {
        this.ViewType = viewType;
    }
}
