using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

/// <summary>
/// Base class derived by <see cref="Point3D"/> and <see cref="Vector3D"/>.
/// Presenting a coordinate in 3-dimensional space.
/// </summary>
public abstract class Coordinate3D
{
    /// <summary>
    /// The X coordinate.
    /// </summary>
    public double X { get; }
    
    /// <summary>
    /// The Y coordinate.
    /// </summary>
    public double Y { get; }
    
    /// <summary>
    /// The Z coordinate.
    /// </summary>
    public double Z { get; }
    
    /// <summary>
    /// Default constructor method for <see cref="Coordinate3D"/>. <br />
    /// Requires X, Y, Z coordinate to be specified explicitly.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    protected Coordinate3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>
    /// Validate value equality for current coordinate and another coordinate.
    /// </summary>
    /// <param name="other">Another <see cref="Coordinate3D"/> object to be compared.</param>
    /// <returns></returns>
    private bool Equals(Coordinate3D other) => X.IsEquivalentTo(other.X) && Y.IsEquivalentTo(other.Y) && Z.IsEquivalentTo(other.Z);

    /// <summary>
    /// Validate value equality for current coordinate and another object with any type.
    /// </summary>
    /// <param name="obj">Another object to be compared.</param>
    /// <returns>
    /// <b>True</b> - if <see cref="obj"/> is a <see cref="Coordinate3D"/> and is value-equal to current coordinate;
    /// <br />
    /// <b>False</b> - otherwise.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Coordinate3D other && GetType() == other.GetType() && Equals(other);
    }

    /// <summary>
    /// Get hashcode for current coordinate.
    /// </summary>
    /// <returns>Hashcode for current coordinate.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override string ToString() => $"({X}, {Y}, {Z})";

    public IEnumerable<double> ToEnumerable() => new[] {X, Y, Z};

    #region static comparation operators
    
    public static bool operator ==(Coordinate3D c1, Coordinate3D c2) => c1.Equals(c2);
    public static bool operator !=(Coordinate3D c1, Coordinate3D c2) => !c1.Equals(c2);
    public static bool operator ==(Coordinate3D c, (double X, double Y, double Z) t) =>
        (c.X, c.Y, c.Z) == t;
    public static bool operator !=(Coordinate3D c, (double X, double Y, double Z) t) =>
        (c.X, c.Y, c.Z) != t;
    public static bool operator ==((double X, double Y, double Z) t, Coordinate3D c) =>
        (c.X, c.Y, c.Z) == t;
    public static bool operator !=((double X, double Y, double Z) t, Coordinate3D c) =>
        (c.X, c.Y, c.Z) != t;
    
    #endregion
    
    #region implicit converters
    
    public static implicit operator (double, double, double)(Coordinate3D c) => (c.X, c.Y, c.Z);
    
    #endregion
    
}