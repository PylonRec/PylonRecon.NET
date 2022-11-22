namespace PylonRecon.Geometry;

public class Segment2D : LineBase2D
{
    public Point2D StartPoint => FixedPoint;
    public Point2D EndPoint { get; }
    public double Length { get; }

    private Line2D? _correspondingLine;
    public override Line2D CorrespondingLine => _correspondingLine ??= new(FixedPoint, DirectionVector);

    public Segment2D(Point2D startPoint, Point2D endPoint) : base(startPoint, startPoint.VectorTo(endPoint), 0d,
        startPoint.DistanceTo(endPoint))
    {
        EndPoint = endPoint;
        Length = startPoint.DistanceTo(endPoint);
    }
}