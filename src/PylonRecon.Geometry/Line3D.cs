namespace PylonRecon.Geometry;

public sealed class Line3D : LineBase3D
{
    #region constants

    private static Line3D? _xAxis;
    public static Line3D XAxis => _xAxis ??= new(Point3D.Origin, new Vector3D(1d, 0d, 0d));

    private static Line3D? _yAxis;
    public static Line3D YAxis => _yAxis ??= new(Point3D.Origin, new Vector3D(0d, 1d, 0d));

    private static Line3D? _zAxis;
    public static Line3D ZAxis => _zAxis ??= new(Point3D.Origin, new Vector3D(0d, 0d, 1d));
    
    #endregion

    public override Line3D CorrespondingLine => this;
    
    public Line3D(Point3D fixedPoint, Vector3D direction) : base(fixedPoint, direction)
    {
        // Do nothing else.
    }

    public Line3D(Point3D onePoint, Point3D anotherPoint) : base(onePoint, onePoint.VectorTo(anotherPoint))
    {
        // Do nothing else.
    }

    public double IncludedAngleWith(Plane3D plane) => plane.IncludedAngleWith(this);

    public double DistanceTo(Point3D point)
    {
        var fp = FixedPoint.VectorTo(point);
        return fp.Length * Math.Sqrt(1d - Math.Pow(fp * DirectionVector / fp.Length, 2d));
    }

    public bool IsParallelTo(Plane3D plane) => plane.IsParallelTo(this);

    public bool IsPerpendicularTo(Plane3D plane) => plane.IsPerpendicularTo(this);
    
    public double? DistanceTo(Line3D other) => IsParallelTo(other) ? DistanceTo(other.FixedPoint) : null;

    public double? DistanceTo(Plane3D plane) => plane.DistanceTo(this);

    public Line3D? ProjectTo(Plane3D plane) => IsPerpendicularTo(plane)
        ? null
        : new Line3D(FixedPoint.ProjectTo(plane), FixedPoint.MoveBy(DirectionVector).ProjectTo(plane));
}