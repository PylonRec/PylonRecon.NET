namespace PylonRecon.Geometry;

public interface IInterpolatable
{
    public IEnumerable<Point3D> GetInterpolatedSample(double interpolationSpacing);
}