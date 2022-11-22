namespace PylonRecon.Geometry;

/// <summary>
/// Half line (ray) in 3-dimensional space.
/// Fixed on one end, which is called <see cref="StartPoint"/>.
/// </summary>
public sealed class HalfLine3D : LineBase3D
{
    /// <summary>
    /// The fixed end of the half line.
    /// </summary>
    public Point3D StartPoint => FixedPoint;

    private Line3D? _correspondingLine;
    /// <summary>
    /// Returns the line that covers the current half line.
    /// </summary>
    public override Line3D CorrespondingLine => _correspondingLine ??= new Line3D(FixedPoint, DirectionVector);

    /// <summary>
    /// Default constructor for the half line.
    /// A fixed point (stored as the <see cref="StartPoint"/>) and the half line's direction vector need to be provided.
    /// </summary>
    /// <param name="startPoint">Start point of the half line.</param>
    /// <param name="directionVector">Direction vector of the half line.
    /// Note: The vector indicates the direction that starts from the start point and expand to infinity.</param>
    public HalfLine3D(Point3D startPoint, Vector3D directionVector) : base(startPoint, directionVector, 0d)
    {
        // Do nothing else.
    }

    /// <summary>
    /// Two-point constructor for the half line.
    /// Start point and another point need to be provided.
    /// </summary>
    /// <param name="startPoint">Start point of the half line.</param>
    /// <param name="anotherPoint"></param>
    public HalfLine3D(Point3D startPoint, Point3D anotherPoint)
        : base(startPoint, startPoint.VectorTo(anotherPoint), 0d)
    {
        // Do nothing else.
    }
}