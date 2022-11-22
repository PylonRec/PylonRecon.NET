using PylonRecon.Algorithm;
using PylonRecon.Geometry;

namespace PylonRecon;

public static class LateralFaceSegmentDivider
{
    public static List<Plane3D> SegmentCloudWithCornerPoints(PointCloud inputCloud,
        List<Point3D> cornerPoints)
    {
        if (cornerPoints.Count != 4)
            throw new ArithmeticException(
                "Make sure 4 corner points are provided to determine the pylon's 4 lateral faces.");
        List<Plane3D> result = new();
        for (int i = 0; i < 4; i++)
        {
            Point3D p1 = cornerPoints[i], p2 = cornerPoints[(i + 1) % 4];
            Point3D center = p1.MoveBy(p1.VectorTo(p2) * 0.5d);
            Plane3D perpendicularPlane = new(center, p1.VectorTo(p2));
            Vector3D xAxis = perpendicularPlane.NormalVector.GetPerpendicularVectorSample().Normalize();
            Vector3D yAxis = (perpendicularPlane.NormalVector ^ xAxis).Normalize();
            var locationNormalMapping =
                inputCloud.Locations.Select(p => p.VectorTo(p1).Normalize() ^ p.VectorTo(p2).Normalize());
            double accuracy = 0.002d;
            var clusters = new List<Vector3D>[(int)Math.Ceiling(2 * Math.PI / accuracy)];
            for (var j = 0; j < clusters.Length; j++)
            {
                clusters[j] = new();
            }
            foreach (var normal in locationNormalMapping)
            {
                var theta = Math.Atan2(normal * yAxis, normal * xAxis);
                clusters[(int) Math.Floor((theta + Math.PI) / accuracy)].Add(normal);
            }
            var targetCluster = clusters.MaxBy(c => c.Count);
            var targetPlaneNormal = targetCluster!.GroupBy(v => v).MaxBy(g => g.Count()).Key;
            result.Add(new(center, targetPlaneNormal));
        }
        return result;
    }
}