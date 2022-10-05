using System.Collections;
using System.Drawing;
using PylonRecon.Geometry;

namespace PylonRecon;

public sealed class PointCloud : IEnumerable<CloudPoint>
{
    public PointCloud(IEnumerable<Point3D> locations)
    {
        _points = locations.Select(p => new CloudPoint()
        {
            Location = p,
            Color = Color.Black,
            Intensity = 0d,
            Normal = Vector3D.Zero
        }).ToList();
    }
    
    public PointCloud(IEnumerable<CloudPoint> points)
    {
        _points = points.ToList();
    }
    
    private readonly List<CloudPoint> _points;

    public IEnumerable<Point3D> Locations => _points.Select(static p => p.Location);

    public IEnumerable<Color> Colors => _points.Select(static p => p.Color);

    public IEnumerable<Vector3D> Normals => _points.Select(static p => p.Normal);

    public IEnumerable<double> Intensities => _points.Select(static p => p.Intensity);

    public (double, double) XLimits => (Locations.Min(static p => p.X), Locations.Max(static p => p.X));
    
    public (double, double) YLimits => (Locations.Min(static p => p.Y), Locations.Max(static p => p.Y));
    
    public (double, double) ZLimits => (Locations.Min(static p => p.Z), Locations.Max(static p => p.Z));
    
    public IEnumerator<CloudPoint> GetEnumerator() => _points.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();
}