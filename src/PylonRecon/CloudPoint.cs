using System.Drawing;
using PylonRecon.Geometry;

namespace PylonRecon;

public sealed class CloudPoint
{
    public Point3D Location { get; set; } = Point3D.Origin;
    public Color Color { get; set; } = Color.Black;
    public Vector3D Normal { get; set; } = Vector3D.Zero;
    public double Intensity { get; set; } = 0d;
}