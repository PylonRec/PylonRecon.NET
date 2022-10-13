using PylonRecon.Geometry;

namespace PylonRecon.Algorithm;

public static class GeometricClusteringAlgorithm
{
    public static List<List<Point3D>> ClusterPoints(List<Point3D> points)
    {
        List<List<Point3D>> result = new();
        double threshold = 0d;
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = 0; j < points.Count; j++)
            {
                if (i == j) continue;
                threshold += points[i].DistanceTo(points[j]);
            }
        }
        threshold /= ((points.Count * points.Count - points.Count) * 100d);
        
        foreach (var point in points)
        {
            var cluster = result.FirstOrDefault(c => c.Any(p => p.DistanceTo(point) < threshold));
            if (cluster is not null) cluster.Add(point);
            else result.Add(new List<Point3D> {point});
        }

        return result;
    }
}