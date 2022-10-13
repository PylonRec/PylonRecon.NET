using System.Collections;
using System.Drawing;
using PylonRecon.Geometry;

namespace PylonRecon;

public sealed class PointCloud : IList<CloudPoint>
{
    public PointCloud()
    {
        _points = new();
    }
    
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

    public void Add(CloudPoint item) => _points.Add(item);

    public void AddRange(IEnumerable<CloudPoint> items) => _points.AddRange(items);

    public void Clear() => _points.Clear();

    public bool Contains(CloudPoint item) => _points.Contains(item);

    public void CopyTo(CloudPoint[] array, int arrayIndex) => _points.CopyTo(array, arrayIndex);
    
    public bool Remove(CloudPoint item) => _points.Remove(item);

    public int Count => _points.Count;
    
    public bool IsReadOnly => false;
    
    public int IndexOf(CloudPoint item) => _points.IndexOf(item);

    public void Insert(int index, CloudPoint item) => _points.Insert(index, item);

    public void RemoveAt(int index) => _points.RemoveAt(index);

    public CloudPoint this[int index]
    {
        get => _points[index];
        set => _points[index] = value;
    }

    public IEnumerator<CloudPoint> GetEnumerator() => _points.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();
}