using System.Drawing;
using PylonRecon.Geometry;

namespace PylonRecon;

public sealed class CloudPoint
{
    public Point3D Location { get; init; } = Point3D.Origin;
    public Color Color { get; init; }
    public Vector3D Normal { get; init; } = Vector3D.Zero;
    public double Intensity { get; init; }
}