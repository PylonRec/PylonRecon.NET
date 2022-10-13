using MathNet.Numerics.LinearAlgebra;

namespace PylonRecon.Geometry.Helpers;

/// <summary>
/// Convex hull generating static helper, used to generate a list collection of points inside the given set,
/// which requires all the points lie in the same plane.
/// </summary>
public static class ConvexHullHelper
{
    public static List<Point3D> ComputeConvexHull(IEnumerable<Point3D> pointSet)
    {
        var points = pointSet.Distinct().ToArray();
        // 2 points don't determine a plane.
        if (points.Length < 3)
            throw new ArithmeticException(
                "To generate a convex hull, the point set must consist of at least 3 points.");
        int planeBuildPointIndex = 2;
        Line3D line01 = new(points[0], points[1]);
        while (line01.Contains(points[planeBuildPointIndex]))
        {
            planeBuildPointIndex++;
        }
        Plane3D plane = new(points[0], points[1], points[planeBuildPointIndex]);
        
        // Compute a set of base on the plane.
        Vector3D xBase = plane.NormalVector switch
        {
            {X: 0} => (1d, 0d, 0d),
            {Y: 0} => (0d, 1d, 0d),
            {Z: 0} => (0d, 0d, 1d),
            _ => (1d, 1d, -1 * (plane.NormalVector.X + plane.NormalVector.Y) / plane.NormalVector.Z)
        };
        Vector3D yBase = plane.NormalVector ^ xBase;

        // Map each valid point in the plane to its relative coordinate.
        Dictionary<Point2D, Point3D> mappedPoints = new();
        foreach (var p in points)
        {
            // Exclude all points that don't lie in the plane.
            if (!plane.Contains(p)) continue;
            var relative = plane.CenterPoint.VectorTo(p).GetRelativeCoordinate(xBase, yBase);
            if (relative is null) continue;
            mappedPoints[relative] = p;
        }

        var relPoints = mappedPoints.Keys.ToList();
        var start = relPoints.MinBy(static p => p.Y);
        if (start is null) throw new ArithmeticException("Cannot decide where to start the convex hull.");
        relPoints.Remove(start);
        
        // Convert coordinates of the points from Cartesian to polar.
        (double Theta, double Rho, Point2D Point)[] polarPoints = relPoints.Select(p =>
        {
            var relative = start.VectorTo(p);
            return (Math.Acos(relative * (1, 0) / relative.Length), relative.Length, p);
        }).OrderBy(c => c.Item1).ThenBy(c => c.Length).ToArray();

        Stack<Point2D> pointStack = new();
        pointStack.Push(start);
        pointStack.Push(polarPoints[0].Point);

        int index = 1;
        while (index < polarPoints.Length)
        {
            if (pointStack.Count == 1)
            {
                pointStack.Push(polarPoints[index].Point);
                index++;
                continue;
            }
            var b = pointStack.Pop();
            var a = pointStack.Peek();
            var ab = a.VectorTo(b);
            var ac = a.VectorTo(polarPoints[index].Point);
            if (ab.X * ac.Y - ac.X * ab.Y > 0.00001)
            {
                pointStack.Push(b);
                pointStack.Push(polarPoints[index].Point);
                index++;
            }
        }

        // Restore relative 2D points back to 3D using mapping.
        return pointStack.Select(p => mappedPoints[p]).ToList();
    }

    private static Point2D? GetRelativeCoordinate(this Vector3D v, Vector3D i, Vector3D j)
    {
        // (v) = (a, b, c), (i) = (l, m, n), (j) = (p, q, r)
        // Solve: (v) = x (i) + y (j)
        if (Matrix<double>.Build.DenseOfColumns(new[]
            {
                i.ToEnumerable(),
                j.ToEnumerable()
            }).Solve(Vector<double>.Build.DenseOfEnumerable(v.ToEnumerable())) is { Count: 2 } result)
        {
            return (result[0], result[1]);
        }
        return null;
    }
}