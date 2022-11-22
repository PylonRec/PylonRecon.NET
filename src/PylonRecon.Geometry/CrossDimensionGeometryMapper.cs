namespace PylonRecon.Geometry;

public class CrossDimensionGeometryMapper
{
    private readonly Point3D _planeOrigin;
    private readonly Vector3D _xAxisPresentedIn3D, _yAxisPresentedIn3D;

    public CrossDimensionGeometryMapper(Point3D planeOrigin, Vector3D xAxisPresentedIn3D, Vector3D yAxisPresentedIn3D)
    {
        _planeOrigin = planeOrigin;
        _xAxisPresentedIn3D = xAxisPresentedIn3D;
        _yAxisPresentedIn3D = yAxisPresentedIn3D;
    }

    public Point2D Map(Point3D point)
    {
        var relative = _planeOrigin.VectorTo(point);
        return (relative * _xAxisPresentedIn3D, relative * _yAxisPresentedIn3D);
    }

    public Point3D MapBack(Point2D point) =>
        _planeOrigin.MoveBy(point.X * _xAxisPresentedIn3D).MoveBy(point.Y * _yAxisPresentedIn3D);

    public Line2D Map(Line3D line)
    {
        var pMapped = Map(line.FixedPoint);
        var qMapped = Map(line.FixedPoint.MoveBy(line.DirectionVector));
        return new(pMapped, qMapped);
    }

    public Line3D MapBack(Line2D line)
    {
        var pMappedBack = MapBack(line.FixedPoint);
        var qMappedBack = MapBack(line.FixedPoint.MoveBy(line.DirectionVector));
        return new(pMappedBack, qMappedBack);
    }
}