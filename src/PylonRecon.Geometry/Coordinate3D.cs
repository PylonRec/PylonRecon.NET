using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

/// <summary>
/// Base class derived by <see cref="Point3D"/> and <see cref="Vector3D"/>.
/// Presenting a coordinate in 3-dimensional space.
/// </summary>
public abstract class Coordinate3D
{
    public double X { get; }
    public double Y { get; }
    public double Z { get; }
    
    /// <summary>
    /// Default constructor method for <see cref="Coordinate3D"/>. <br />
    /// Requires X, Y, Z coordinate to be specified explicitly.
    /// </summary>
    protected Coordinate3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>
    /// Validate value equality for current coordinate and another.
    /// </summary>
    private bool Equals(Coordinate3D other) => X.IsEquivalentTo(other.X) && Y.IsEquivalentTo(other.Y) && Z.IsEquivalentTo(other.Z);

    /// <summary>
    /// Validate value equality for current coordinate and another object with any type.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Coordinate3D other && GetType() == other.GetType() && Equals(other);
    }

    /// <summary>
    /// Get hashcode for current coordinate.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <summary>
    /// Default string that represents current coordinate.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// 
    /// </summary>
    public string ToString(char separator) => $"{X}{separator}{Y}{separator}{Z}";

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