namespace PylonRecon.Geometry;

public class CoordinateSystemConverter3D
{
    private readonly CoordinateSystem3D _from;
    private readonly CoordinateSystem3D _to;

    public CoordinateSystemConverter3D(CoordinateSystem3D from, CoordinateSystem3D to)
    {
        _from = from;
        _to = to;
    }

    public IEnumerable<Point3D> Convert(IEnumerable<Point3D> points)
    {
        foreach (var point in points)
        {
            var absolute =
                _from.SystemOrigin
                    .MoveBy(point.X * _from.VectorOx)
                    .MoveBy(point.Y * _from.VectorOy)
                    .MoveBy(point.Z * _from.VectorOz);
            var relative = _to.SystemOrigin.VectorTo(absolute);
            yield return (relative * _to.VectorOx, relative * _to.VectorOy, relative * _to.VectorOz);
        }
    }
    
    public IEnumerable<Point3D> ConvertBack(IEnumerable<Point3D> points)
    {
        foreach (var point in points)
        {
            var absolute =
                _to.SystemOrigin
                    .MoveBy(point.X * _to.VectorOx)
                    .MoveBy(point.Y * _to.VectorOy)
                    .MoveBy(point.Z * _to.VectorOz);
            var relative = _from.SystemOrigin.VectorTo(absolute);
            yield return (relative * _from.VectorOx, relative * _from.VectorOy, relative * _from.VectorOz);
        }
    }
}