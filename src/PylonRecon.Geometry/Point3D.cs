namespace PylonRecon.Geometry;

/// <summary>
/// Point type in 3-dimensional space.
/// </summary>
public sealed class Point3D : Coordinate3D
{
    #region constants
    
    private static Point3D? _origin;
    /// <summary>
    /// The Origin point (0, 0, 0) in XYZ coordinate system.
    /// </summary>
    public static Point3D Origin => _origin ??= (0d, 0d, 0d);
    
    #endregion

    /// <summary>
    /// Default constructor for <see cref="Point3D"/> object. <br />
    /// Requires X, Y, Z coordinate to be specified explicitly.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    public Point3D(double x, double y, double z) : base(x, y, z)
    {
        // Do nothing else.
    }

    #region implicit converters
    
    public static implicit operator Point3D((double X, double Y, double Z) t) => new(t.X, t.Y, t.Z);

    #endregion

    /// <summary>
    /// Computes the vector starting from current point to another point.
    /// </summary>
    /// <param name="end">The end of the vector.</param>
    /// <returns>A <see cref="Vector3D"/> object indicating the vector
    /// starting from the current point to the specified end.</returns>
    public Vector3D VectorTo(Point3D end) => (end.X - X, end.Y - Y, end.Z - Z);
    
    /// <summary>
    /// Computes the distance between the current point and the specified point.
    /// </summary>
    /// <param name="other">Another point specified.</param>
    /// <returns>Distance between current point and the specified point.</returns>
    public double DistanceTo(Point3D other) => VectorTo(other).Length;

    /// <summary>
    /// Computes the distance between the current point and the specified line.
    /// </summary>
    /// <param name="line">The specified line.</param>
    /// <returns>Distance between current point and the specified line.</returns>
    public double DistanceTo(Line3D line) => line.DistanceTo(this);

    /// <summary>
    /// Computes the distance between the current point and the specified plane.
    /// </summary>
    /// <param name="plane">The specified plane.</param>
    /// <returns>Distance between current point and the specified plane.</returns>
    public double DistanceTo(Plane3D plane) => plane.DistanceTo(this);
    
    /// <summary>
    /// Computes target point of current point moved by specified displacement. <br />
    /// Coordinate of current point won't be affected.
    /// </summary>
    /// <param name="displacement">Displacement vector of the move.</param>
    /// <returns>The target point of the move.</returns>
    public Point3D MoveBy(Vector3D displacement) => (X + displacement.X, Y + displacement.Y, Z + displacement.Z);

    /// <summary>
    /// Returns a random point whose coordinates all lie between -1 and 1.
    /// </summary>
    /// <returns>The random point constructed.</returns>
    public static Point3D Random() => Origin.MoveBy(Vector3D.Random());
    
    /// <summary>
    /// Returns a random point whose coordinates all lie between min and max.
    /// </summary>
    /// <param name="min">Minimum limit of the coordinates.</param>
    /// <param name="max">Maximum limit of the coordinates.</param>
    /// <returns>The random point constructed.</returns>
    public static Point3D Random(double min, double max) => Origin.MoveBy(Vector3D.Random(min, max));
}