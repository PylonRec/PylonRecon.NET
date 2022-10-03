namespace PylonRecon.Geometry;

public class CoordinateSystemConverter3D
{
    public CoordinateSystem3D From { get; set; }
    public CoordinateSystem3D To { get; set; }

    public IEnumerable<Point3D> Convert(IEnumerable<Point3D> points)
    {
        yield return null;
    }
    
    public IEnumerable<Point3D> ConvertBack(IEnumerable<Point3D> points)
    {
        yield return null;
    }
}