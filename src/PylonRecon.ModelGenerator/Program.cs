using PylonRecon;
using PylonRecon.Geometry;
using PylonRecon.IO;

Random rand = new();

var centralPoint = Point3D.Random(0, 1000);

Vector3D topDirection = (0d, 0d, 1d);
// var topDirection = Vector3D.Random().Normalize();
Vector3D frontDirection = topDirection.GetPerpendicularVectorSample();
Vector3D leftDirection = frontDirection ^ topDirection;

double halfHeight = rand.NextDouble() * 400d + 1800d; // (1800, 2200)
double halfTopWidth = rand.NextDouble() * 400d + 200d; // (200, 600)
double halfTopDepth = rand.NextDouble() * 400d + 200d; // (200, 600)
double halfBottomWidth = rand.NextDouble() * 400d + 800d; // (800, 1200)
double halfBottomDepth = rand.NextDouble() * 400d + 800d; // (800, 1200)

// 1: top, left, front; 0: bottom, right, back
// i.e. P5 - 101 - point on top right front corner
// Top/Bottom: move by +-1 * topDirection * halfHeight
// Left/Right: move by +-1 * leftDirection * halfWidth
// Front/Back: move by +-1 * frontDirection * halfDepth
Point3D[] cornerPoints = new Point3D[8];
for (var i = 0; i < 8; i++)
{
    double heightFactor = 4 == (i & 4) ? 1d : -1d;
    double widthFactor = 2 == (i & 2) ? 1d : -1d;
    double depthFactor = 1 == (i & 1) ? 1d : -1d;
    double halfWidth = 4 == (i & 4) ? halfTopWidth : halfBottomWidth;
    double halfDepth = 4 == (i & 4) ? halfTopDepth : halfBottomDepth;
    cornerPoints[i] = centralPoint
        .MoveBy(heightFactor * topDirection * halfHeight)
        .MoveBy(widthFactor * leftDirection * halfWidth)
        .MoveBy(depthFactor * frontDirection * halfDepth);
}

(int BottomEnd, int TopEnd)[] sideEdgeIndices = {(0, 4), (1, 5), (2, 6), (3, 7)};
(int Edge1, int Edge2)[] faceIndices = {(0, 1), (1, 3), (3, 2), (2, 0)};

PointCloud cloud = new();

int sideSegments = rand.Next(6, 12);

foreach (var face in faceIndices)
{
    var edge1 = sideEdgeIndices[face.Edge1];
    var edge2 = sideEdgeIndices[face.Edge2];
    List<(Point3D End1, Point3D End2)> edges = new()
    {
        (cornerPoints[edge1.TopEnd], cornerPoints[edge1.BottomEnd]),
        (cornerPoints[edge1.TopEnd], cornerPoints[edge2.TopEnd]),
        (cornerPoints[edge1.BottomEnd], cornerPoints[edge2.BottomEnd])
    };
    var edge1Unit = cornerPoints[edge1.BottomEnd].VectorTo(cornerPoints[edge1.TopEnd]) * (1d / sideSegments);
    var edge2Unit = cornerPoints[edge2.BottomEnd].VectorTo(cornerPoints[edge2.TopEnd]) * (1d / sideSegments);
    for (int i = 1; i < sideSegments; i++)
    {
        edges.Add((cornerPoints[edge1.BottomEnd].MoveBy((i - 0.5d) * edge1Unit),
            cornerPoints[edge2.BottomEnd].MoveBy((i + 0.5d) * edge2Unit)));
        edges.Add((cornerPoints[edge2.BottomEnd].MoveBy((i - 0.5d) * edge2Unit),
            cornerPoints[edge1.BottomEnd].MoveBy((i + 0.5d) * edge1Unit)));
    }
    var midTop = cornerPoints[edge1.TopEnd]
        .MoveBy(cornerPoints[edge1.TopEnd].VectorTo(cornerPoints[edge2.TopEnd]) * 0.5d);
    var midBottom = cornerPoints[edge1.BottomEnd]
        .MoveBy(cornerPoints[edge1.BottomEnd].VectorTo(cornerPoints[edge2.BottomEnd]) * 0.5d);
    edges.Add((midTop, cornerPoints[edge1.TopEnd].MoveBy(-0.5d * edge1Unit)));
    edges.Add((midTop, cornerPoints[edge2.TopEnd].MoveBy(-0.5d * edge2Unit)));
    edges.Add((midBottom, cornerPoints[edge1.BottomEnd].MoveBy(0.5d * edge1Unit)));
    edges.Add((midBottom, cornerPoints[edge2.BottomEnd].MoveBy(0.5d * edge2Unit)));
    cloud.AddRange(edges.SelectMany(
        edge => new Cylinder3D(edge.End1, edge.End2, 2d)
            .GetInterpolatedSample(20d)
            .Select(p3d => new CloudPoint {Location = p3d})));
}

XyzPointCloudDataFileWriter writer = new();
writer.WriteTo(cloud, "/Users/brandon/Desktop/model.xyz");

Console.WriteLine($"Center: {centralPoint}");
Console.WriteLine($"Axis: {topDirection}");