namespace PylonRecon.Geometry;

public class HalfLine2D : LineBase2D
{
    public Point2D StartPoint => FixedPoint;

    private Line2D? _correspondingLine;
    public override Line2D CorrespondingLine => _correspondingLine ??= new Line2D(FixedPoint, DirectionVector);

    public HalfLine2D(Point2D startPoint, Vector2D directionVector) : base(startPoint, directionVector, 0d)
    {
        // Do nothing else.
    }

    public HalfLine2D(Point2D startPoint, Point2D anotherPoint) : base(startPoint, startPoint.VectorTo(anotherPoint),
        0d)
    {
        // Do nothing else.
    }
}