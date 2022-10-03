using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public sealed class Vector3D : Coordinate3D
{
    /// <summary>
    /// Default constructor for the vector.
    /// Requires X, Y, Z coordinates to be all specified explicitly.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    public Vector3D(double x, double y, double z) : base(x, y, z)
    {
        // Do nothing else.
    }
    
    #region constants
    
    private static Vector3D? _zero;
    /// <summary>
    /// Zero vector (0, 0, 0).
    /// </summary>
    public static Vector3D Zero => _zero ??= (0d, 0d, 0d);
    
    #endregion
    
    /// <summary>
    /// Length (Norm, Module) of current vector.
    /// </summary>
    public double Length => Math.Sqrt(this * this);

    # region operators
    
    /// <summary>
    /// Sum of two vectors.
    /// </summary>
    public static Vector3D operator +(Vector3D v1, Vector3D v2) => (v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    
    /// <summary>
    /// Difference of two vectors.
    /// </summary>
    public static Vector3D operator -(Vector3D v1, Vector3D v2) => (v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    
    /// <summary>
    /// Scalar product of two vectors.
    /// </summary>
    public static double operator *(Vector3D v1, Vector3D v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    
    /// <summary>
    /// Scalar multiplication of a vector and a number.
    /// </summary>
    public static Vector3D operator *(Vector3D v, double d) => (v.X * d, v.Y * d, v.Z * d);
    
    /// <summary>
    /// Scalar multiplication of a vector and a number.
    /// </summary>
    public static Vector3D operator *(double d, Vector3D v) => v * d;
    
    /// <summary>
    /// Cross product of two vectors.
    /// </summary>
    public static Vector3D operator ^(Vector3D v1, Vector3D v2) => (v1.Y * v2.Z - v1.Z * v2.Y,
        v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);

    # endregion
    
    #region implicit converters
    
    public static implicit operator Vector3D((double X, double Y, double Z) t) => new(t.X, t.Y, t.Z);

    #endregion

    /// <summary>
    /// Computes if current vector is parallel to another. <br />
    /// Note: <see cref="Zero"/> is parallel to any vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>
    /// <b>True</b> - if two vectors are parallel.
    /// <b>False</b> - otherwise.
    /// </returns>
    public bool IsParallelTo(Vector3D other)
    {
        // Using right-hand rule, if two vectors are parallel,
        // their cross product should be equal to (0).
        return (this ^ other) == Zero;
    }

    /// <summary>
    /// Computes if current vector is perpendicular to another. <br />
    /// Note: <see cref="Zero"/> is perpendicular to any vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>
    /// <b>True</b> - if two vectors are perpendicular.
    /// <b>False</b> - otherwise.
    /// </returns>
    public bool IsPerpendicularTo(Vector3D other) => (this * other).IsZero();

    /// <summary>
    /// Normalize current vector and return the normalized result. <br />
    /// Note: This method won't modify data of current object.
    /// </summary>
    /// <returns>The normalized vector, which shares the same direction with current vector,
    /// yet is 1 unit long.</returns>
    public Vector3D Normalize() => 1d / Length * this;

    /// <summary>
    /// Included angle between the current vector and another vector.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double IncludedAngleWith(Vector3D other) => Math.Acos(this * other / Length / other.Length);

    /// <summary>
    /// Returns a random vector whose coordinates all lie between -1 and 1.
    /// </summary>
    /// <returns>The random vector constructed.</returns>
    public static Vector3D Random() => Random(-1, 1);
    
    /// <summary>
    /// Returns a random vector whose coordinates all lie between min and max.
    /// </summary>
    /// <param name="min">Minimum limit of the coordinates.</param>
    /// <param name="max">Maximum limit of the coordinates.</param>
    /// <returns>The random vector constructed.</returns>
    public static Vector3D Random(double min, double max)
    {
        Random random = new();
        return new Vector3D(random.NextDouble(), random.NextDouble(), random.NextDouble()) * (max - min) +
               (min, min, min);
    }
}