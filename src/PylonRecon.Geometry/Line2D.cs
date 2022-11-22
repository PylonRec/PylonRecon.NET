namespace PylonRecon.Geometry;

public class Line2D : LineBase2D
{
    #region constants

    private static Line2D _xAxis;
    public static Line2D XAxis => _xAxis ??= new(Point2D.Origin, new Vector2D(1d, 0d));
    
    private static Line2D _yAxis;
    public static Line2D YAxis => _yAxis ??= new(Point2D.Origin, new Vector2D(0d, 1d));
    
    #endregion

    public override Line2D CorrespondingLine => this;

    public Line2D(Point2D fixedPoint, Vector2D direction) : base(fixedPoint, direction)
    {
        // Do nothing else.
    }

    public Line2D(Point2D onePoint, Point2D anotherPoint) : base(onePoint, onePoint.VectorTo(anotherPoint))
    {
        // Do nothing else.
    }

    public double DistanceTo(Point2D point)
    {
        var fp = FixedPoint.VectorTo(point);
        return fp.Length * Math.Sqrt(1 - Math.Pow(fp * DirectionVector / fp.Length, 2d));
    }

    public double? DistanceTo(Line2D other) => IsParallelTo(other) ? DistanceTo(other.FixedPoint) : null;
}