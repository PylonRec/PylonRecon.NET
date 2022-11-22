using PylonRecon.Geometry;

namespace PylonRecon.Algorithm;

public static class GeometricClusteringAlgorithm
{
    public static List<List<Point3D>> ClusterPoints(List<Point3D> points, double thresholdRatio)
    {
        double threshold = 0d;
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = 0; j < points.Count; j++)
            {
                if (i == j) continue;
                threshold += points[i].DistanceTo(points[j]);
            }
        }
        threshold /= points.Count * points.Count - points.Count;
        threshold *= thresholdRatio;
        return ClusteringAlgorithm<Point3D>.Cluster(points, (p, q) => p.DistanceTo(q) < threshold);
    }
    
    public static List<List<Point2D>> ClusterPoints(List<Point2D> points, double thresholdRatio)
    {
        double threshold = 0d;
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = 0; j < points.Count; j++)
            {
                if (i == j) continue;
                threshold += points[i].DistanceTo(points[j]);
            }
        }
        threshold /= points.Count * points.Count - points.Count;
        threshold *= thresholdRatio;
        return ClusteringAlgorithm<Point2D>.Cluster(points, (p, q) => p.DistanceTo(q) < threshold);
    }

    public static List<List<Vector3D>> ClusterVectors(IEnumerable<Vector3D> vectors, double thresholdIncludedAngle) =>
        ClusteringAlgorithm<Vector3D>.Cluster(vectors,
            (u, v) => Math.Acos(Math.Abs(u.Normalize() * v.Normalize())) < thresholdIncludedAngle);
}