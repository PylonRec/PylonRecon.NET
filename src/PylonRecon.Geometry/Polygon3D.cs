namespace PylonRecon.Geometry;

public class Polygon3D : IInterpolatable
{
    private List<Point3D> _convexHull;

    public Polygon3D(IEnumerable<Point3D> pointSet)
    {
        _convexHull = Helpers.ConvexHullHelper.ComputeConvexHull(pointSet);
    }
    
    public IEnumerable<Point3D> GetInterpolatedSample(double interpolationSpacing)
    {
        List<Point3D> result = new();
        for (int i = 0; i + 2 < _convexHull.Count; i++)
        {
            result.AddRange(
                new Triangle3D(_convexHull[i], _convexHull[i + 1], _convexHull[_convexHull.Count - 1])
                    .GetInterpolatedSample(interpolationSpacing));
        }
        return result;
    }
}