using System.Net.Http.Headers;
using PylonRecon;
using PylonRecon.Algorithm;
using PylonRecon.Geometry;
using PylonRecon.IO;

XyzDocumentReader reader = new();
XyzDocumentWriter writer = new();

string root = "/Users/brandon/Desktop";

var cloud = reader.ReadFrom(Path.Join(root, "model.xyz"));

// CentralAxisFinder centralAxisFinder = new(cloud);

// var centralAxis = centralAxisFinder.FindCentralAxis(50, 0.6, 0.2, 0.4, 30);

var centralAxis = Line3D.ZAxis;

PylonBodyBottomCornerExtractor bodyBottomCornerExtractor = new(cloud, centralAxis);

var cornerPoints = bodyBottomCornerExtractor.Extract();

writer.WriteTo(new(cornerPoints), Path.Join(root, "Corner.xyz"));

var lateralFaces = LateralFaceSegmentDivider.SegmentCloudWithCornerPoints(cloud, cornerPoints);

List<Line3D> lateralEdges = new();

for (var i = 0; i < 4; i++)
{
    lateralEdges.Add(lateralFaces[(i + 3) % 4].IntersectionLineWith(lateralFaces[i]));
}

List<Point3D> targetFeaturePoints = new();
List<Point3D> intermediate = new();

for (var i = 0; i < 4; i++)
{
    var faceCloud = CloudDividerByPlane.SegmentCloud(cloud, lateralFaces[i], 2);
    Point3D leftBottomCorner = cornerPoints[i], rightBottomCorner = cornerPoints[(i + 1) % 4];
    Point3D centerBottomPoint = Point3D.GeometricCenterOf(leftBottomCorner, rightBottomCorner);
    Vector3D xAxis = leftBottomCorner.VectorTo(rightBottomCorner).Normalize();
    Vector3D yAxis = lateralFaces[i].NormalVector ^ xAxis;
    CrossDimensionGeometryMapper mapper = new(centerBottomPoint, xAxis, yAxis);
    
    Line2D leftLateralEdge = mapper.Map(lateralEdges[i]), rightLateralEdge = mapper.Map(lateralEdges[(i + 1) % 4]);

    List<Point2D> projected = faceCloud.Locations.Select(p => mapper.Map(p.ProjectTo(lateralFaces[i]))).ToList();

    var centerLineNeighboringPoints = projected.Where(p => p.DistanceTo(Line2D.YAxis) < 100).ToList();
    var centerLinePoints = GeometricClusteringAlgorithm.ClusterPoints(centerLineNeighboringPoints, 0.1d)
        .Select(cluster => new Point2D(cluster.Average(p => p.X), cluster.Average(p => p.Y)));

    intermediate.AddRange(centerLinePoints.Select(mapper.MapBack));
    
    foreach (var centerLinePoint in centerLinePoints)
    {
        var neighbors = projected.Where(p => p.DistanceTo(centerLinePoint) < 100);
        var lines = HoughTransformLineFitter.FitLines(centerLinePoint, neighbors, 2, 0.1d);
        targetFeaturePoints.AddRange(lines.Select(line => mapper.MapBack(line.IntersectionPointWith(leftLateralEdge))));
        targetFeaturePoints.AddRange(lines.Select(line => mapper.MapBack(line.IntersectionPointWith(rightLateralEdge))));
    }
}

var result = GeometricClusteringAlgorithm.ClusterPoints(targetFeaturePoints, 0.05d).Select(cluster =>
{
    var x = cluster.Average(p => p.X);
    var y = cluster.Average(p => p.Y);
    var z = cluster.Average(p => p.Z);
    return new Point3D(x, y, z);
});

writer.WriteTo(new(intermediate), Path.Join(root, "Intermediate.xyz"));
writer.WriteTo(new(result), Path.Join(root, "ResultCloud.xyz"));