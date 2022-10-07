using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

/// <summary>
/// Base class derived by <see cref="Point2D"/> ans <see cref="Vector2D"/>.
/// Presenting a coordinate in 2-dimensional space.
/// </summary>
public abstract class Coordinate2D
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
    /// Default constructor method for <see cref="Coordinate2D"/>.
    /// Requires X, Y coordinates to be specified explicitly.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    protected Coordinate2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Validate value equality for current coordinate and another coordinate.
    /// </summary>
    /// <param name="other">Another <see cref="Coordinate2D"/> object to be compared.</param>
    /// <returns></returns>
    private bool Equals(Coordinate2D other) => X.IsEquivalentTo(other.X) && Y.IsEquivalentTo(other.Y);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Coordinate2D other && GetType() == other.GetType() && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"({X}, {Y})";

    public IEnumerable<double> ToEnumerable() => new[] {X, Y};
    
    #region static comparation operators

    public static bool operator ==(Coordinate2D c1, Coordinate2D c2) => c1.Equals(c2);
    public static bool operator !=(Coordinate2D c1, Coordinate2D c2) => !c1.Equals(c2);
    public static bool operator ==(Coordinate2D c, (double X, double Y) t) => (c.X, c.Y) == t;
    public static bool operator !=(Coordinate2D c, (double X, double Y) t) => (c.X, c.Y) != t;
    public static bool operator ==((double X, double Y) t, Coordinate2D c) => (c.X, c.Y) == t;
    public static bool operator !=((double X, double Y) t, Coordinate2D c) => (c.X, c.Y) != t;

    #endregion
    
    #region implicit converters

    public static implicit operator (double, double)(Coordinate2D c) => (c.X, c.Y);
    
    #endregion
}