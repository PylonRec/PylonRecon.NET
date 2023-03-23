using System;
using PylonRecon.Geometry;

namespace PylonRecon.Shared;

public class AxisFixBridge
{
    private static AxisFixBridge? _instance;
    public static AxisFixBridge Instance => _instance ??= new();

    public Func<Vector3D>? CaptureAxisDirectionFunc { private get; set; }
    public Vector3D? CaptureAxisDirection()
    {
        if (CaptureAxisDirectionFunc is null) return null;
        return CaptureAxisDirectionFunc();
    }
}