using PylonRecon.Algorithm;
using PylonRecon.Geometry;
using PylonRecon.Geometry.Helpers;

namespace PylonRecon;

public class PylonBodyBottomCornerExtractor
{
    private readonly PointCloud _cloud;
    private readonly Line3D _centralAxis;

    public PylonBodyBottomCornerExtractor(PointCloud cloud, Line3D centralAxis)
    {
        _cloud = cloud;
        _centralAxis = centralAxis;
    }
    
    public List<Point3D> Extract()
    {
        var projectionPlane = _centralAxis.SamplePerpendicularPlane();
        var projected = _cloud.Locations.Select(p => (p.ProjectTo(projectionPlane), p)).ToList();
        var convexHull = ConvexHullHelper.ComputeConvexHull(projected.Select(p => p.Item1));
        var clusters = GeometricClusteringAlgorithm.ClusterPoints(convexHull);
        List<Point3D> result = new();
        foreach (var cluster in clusters)
        {
            var restoredPoints = cluster.Select(p => projected.First(q => q.Item1 == p).p);
            double x = restoredPoints.Average(p => p.X);
            double y = restoredPoints.Average(p => p.Y);
            double z = restoredPoints.Average(p => p.Z);
            result.Add((x, y, z));
        }
        return result;
    }
}