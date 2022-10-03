using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public abstract class Coordinate2D
{
    public double X { get; }
    public double Y { get; }

    protected Coordinate2D(double x, double y)
    {
        X = x;
        Y = y;
    }

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