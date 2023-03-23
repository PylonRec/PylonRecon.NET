using HelixToolkit.Wpf;
using PylonRecon.Geometry;
using PylonRecon.Shared;

namespace PylonRecon.Wpf.BridgeOperations;

public class AxisFixOperations
{
    private readonly HelixViewport3D _viewport;

    public AxisFixOperations(HelixViewport3D viewport)
    {
        _viewport = viewport;
    }
    
    public void Bind()
    {
        AxisFixBridge.Instance.CaptureAxisDirectionFunc = CaptureAxisDirection;
    }

    private Vector3D CaptureAxisDirection()
    {
        var d = _viewport.Camera.UpDirection;
        return (d.X, d.Y, d.Z);
    }
}