using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public class Vector2D : Coordinate2D
{
    public Vector2D(double x, double y) : base(x, y)
    {
        // Do nothing else.
    }
    
    #region constants

    private static Vector2D? _zero;
    public static Vector2D Zero => _zero ??= (0d, 0d);
    
    #endregion

    public double Length => Math.Sqrt(this * this);
    
    #region operators

    public static Vector2D operator +(Vector2D v1, Vector2D v2) => (v1.X + v2.X, v1.Y + v2.Y);

    public static Vector2D operator -(Vector2D v1, Vector2D v2) => (v1.X - v2.X, v1.Y - v2.Y);

    public static double operator *(Vector2D v1, Vector2D v2) => v1.X * v2.X + v1.Y * v2.Y;

    public static Vector2D operator *(Vector2D v, double d) => (v.X * d, v.Y * d);

    public static Vector2D operator *(double d, Vector2D v) => (v.X * d, v.Y * d);
    
    #endregion
    
    #region implicit converters

    public static implicit operator Vector2D((double X, double Y) t) => new(t.X, t.Y);
    
    #endregion

    public bool IsParallelTo(Vector2D other) => (X * other.Y).IsEquivalentTo(Y * other.X);

    public bool IsPerpendicularTo(Vector2D other) => (this * other).IsZero();

    public double IncludedAngleWith(Vector2D other) => Math.Acos(this * other / Length / other.Length);
    
    public Vector2D Normalize() => 1d / Length * this;

    public static Vector2D Random() => Random(-1, 1);

    public static Vector2D Random(double min, double max)
    {
        Random random = new();
        return new Vector2D(random.NextDouble(), random.NextDouble()) * (max - min) + (min, min);
    }
}