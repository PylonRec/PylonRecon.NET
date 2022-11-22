using PylonRecon.Algorithm;
using PylonRecon.Algorithm.Helpers;
using PylonRecon.Geometry;
using PylonRecon.Geometry.Helpers;
using PylonRecon.IO;

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
        var convexHull = ConvexHullHelper.ComputeComplexConvexHull(_cloud.Locations, p => p.ProjectTo(projectionPlane));
        convexHull = InterpolateConvexHull(convexHull).ToList();

        List<Point3D> outliers = convexHull.ToList();
        List<Line3D> lines = new();
        RandomSampleConsensusAlgorithm<Point3D, Line3D> lineRansac = new(new Line3DRansacModel());
        int assertion = convexHull.Count / 6;
        int i = 0;
        while (lines.Count < 4)
        {
            var ransacResult = lineRansac.ComputeModel(outliers, 100, 100, assertion);
            if (ransacResult is null) continue;
            lines.Add(ransacResult.Value.Item1);
            PointCloud inlierCloud = new(ransacResult.Value.inliers);
            new XyzDocumentWriter().WriteTo(inlierCloud, $"/Users/brandon/Desktop/Ransac{i++}.xyz");
            foreach (var inlier in ransacResult.Value.inliers)
            {
                outliers.Remove(inlier);
            }
        }

        RandomSampleConsensusAlgorithm<Point3D, Plane3D> planeRansac = new(new Plane3DRansacModel());
        assertion = (int)Math.Floor(convexHull.Count * 0.8d);
        var plane = planeRansac.ComputeModel(convexHull, 100, 10, assertion);

        lines = lines.Select(l => l.ProjectTo(plane!.Value.Item1)!).ToList();

        var lineIndices = new[] {1, 2, 3};
        var parallelLineIndex = lineIndices.MinBy(i => lines[0].IncludedAngleWith(lines[i]));
        var perpLineIndex1 = lineIndices.First(i => i != parallelLineIndex);
        var perpLineIndex2 = lineIndices.Last(i => i != parallelLineIndex);

        List<Point3D> result = new()
        {
            lines[0].IntersectionPointWith(lines[perpLineIndex1])!,
            lines[parallelLineIndex].IntersectionPointWith(lines[perpLineIndex1])!,
            lines[parallelLineIndex].IntersectionPointWith(lines[perpLineIndex2])!,
            lines[0].IntersectionPointWith(lines[perpLineIndex2])!
        };

        return result.Select(p => convexHull.MinBy(q => q.DistanceTo(p))!).ToList();
    }

    private IEnumerable<Point3D> InterpolateConvexHull(List<Point3D> convexHull)
    {
        double minDistance = convexHull[0].DistanceTo(convexHull[1]);
        for (var i = 0; i < convexHull.Count - 1; i++)
        {
            minDistance = Math.Min(minDistance, convexHull[i].DistanceTo(convexHull[i + 1]));
        }
        minDistance *= 2;
        for (var i = 0; i < convexHull.Count - 1; i++)
        {
            Point3D currentPoint = convexHull[i];
            var direction = convexHull[i].VectorTo(convexHull[i + 1]).Normalize();
            while (currentPoint.DistanceTo(convexHull[i + 1]) > minDistance)
            {
                yield return currentPoint;
                currentPoint = currentPoint.MoveBy(minDistance * direction);
            }
        }
        yield return convexHull[^1];
    }
}