using System;

namespace PylonRecon.Shared;

public class BrowseFileBridge
{
    private static BrowseFileBridge? _instance;
    public static BrowseFileBridge Instance => _instance ??= new();

    public Action<string>? FileOpenAction { private get; set; }
    public void OpenFile(string filePath)
    {
        if (FileOpenAction is null) return;
        FileOpenAction(filePath);
    }

    public Action<PointCloud>? PointCloudLoadedAction { private get; set; }
    public void LoadPointCloud(PointCloud cloud)
    {
        if (PointCloudLoadedAction is null) return;
        PointCloudLoadedAction(cloud);
    }
}
