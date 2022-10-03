namespace PylonRecon.Geometry;

public class Point2D : Coordinate2D
{
    #region constants

    private static Point2D? _origin;
    public static Point2D Origin => _origin ??= (0d, 0d);

    #endregion

    public Point2D(double x, double y) : base(x, y)
    {
        // Do nothing else.
    }
    
    #region implicit converters

    public static implicit operator Point2D((double X, double Y) t) => new(t.X, t.Y);

    #endregion

    public Vector2D VectorTo(Point2D end) => (end.X - X, end.Y - Y);
    
    public double DistanceTo(Point2D other) => VectorTo(other).Length;

    public Point2D MoveBy(Vector2D displacement) => (X + displacement.X, Y + displacement.Y);

    public static Point2D Random() => Origin.MoveBy(Vector2D.Random());

    public static Point2D Random(double min, double max) => Origin.MoveBy(Vector2D.Random(min, max));
}