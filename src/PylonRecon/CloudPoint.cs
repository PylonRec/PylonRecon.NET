using System.Drawing;
using PylonRecon.Geometry;

namespace PylonRecon;

public sealed class CloudPoint
{
    public Point3D Location { get; set; }
    public Color Color { get; set; }
    public Vector3D Normal { get; set; }
    public double Intensity { get; set; }
}