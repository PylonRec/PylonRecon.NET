namespace PylonRecon.Geometry;

public sealed class Segment3D : LineBase3D, IInterpolatable
{
    public Point3D StartPoint => FixedPoint;
    public Point3D EndPoint { get; }
    public double Length { get; }
    
    private Line3D? _correspondingLine;
    public override Line3D CorrespondingLine => _correspondingLine ??= new(FixedPoint, DirectionVector);

    public Segment3D(Point3D startPoint, Point3D endPoint)
        : base(startPoint, startPoint.VectorTo(endPoint), 0d, startPoint.DistanceTo(endPoint))
    {
        EndPoint = endPoint;
        Length = startPoint.DistanceTo(endPoint);
    }

    public IEnumerable<Point3D> GetInterpolatedSample(double interpolationSpacing)
    {
        List<Point3D> result = new();
        for (double position = 0d; position < Length; position += interpolationSpacing)
        {
            result.Add(StartPoint.MoveBy(position * DirectionVector));
        }
        result.Add(EndPoint);
        return result;
    }
}