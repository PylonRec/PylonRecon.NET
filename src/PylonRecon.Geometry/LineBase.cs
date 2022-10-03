using MathNet.Numerics.LinearAlgebra;
using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public abstract class LineBase
{
    /// <summary>
    /// Direction vector of the object.
    /// </summary>
    public Vector3D DirectionVector { get; }
    
    /// <summary>
    /// For any <see cref="LineBase"/> object, returns the line that contains this object.
    /// </summary>
    public abstract Line3D CorrespondingLine { get; }

    /// <summary>
    /// The fixed point that limits the position of the object.
    /// </summary>
    internal readonly Point3D FixedPoint;

    /// <summary>
    /// End limits of the object. Used to indicate boundary of the object.
    /// </summary>
    private readonly double? _minPosition, _maxPosition;

    /// <summary>
    /// Default constructor of line base. All data should be provided to construct the object. <br />
    /// All sub-classes should call this constructor method
    /// </summary>
    /// <param name="fixedPoint"></param>
    /// <param name="directionVector"></param>
    /// <param name="minPosition"></param>
    /// <param name="maxPosition"></param>
    /// <exception cref="ArithmeticException"></exception>
    internal LineBase(Point3D fixedPoint, Vector3D directionVector, double? minPosition = null,
        double? maxPosition = null)
    {
        if (directionVector.Length.IsZero())
            throw new ArithmeticException("Lines must be constructed with given direction vectors.");
        DirectionVector = directionVector.Normalize();
        FixedPoint = fixedPoint;
        _minPosition = minPosition;
        _maxPosition = maxPosition;
    }
    
    public bool Contains(Point3D point)
    {
        if (!DirectionVector.IsParallelTo(FixedPoint.VectorTo(point))) return false;
        var relativePosition = FixedPoint.VectorTo(point).Length;
        return (_minPosition is null || relativePosition >= _minPosition) &&
               (_maxPosition is null || relativePosition <= _maxPosition);
    }

    public bool IsParallelTo(LineBase other) => DirectionVector.IsParallelTo(other.DirectionVector);

    public bool IsPerpendicularTo(LineBase other) => DirectionVector.IsPerpendicularTo(other.DirectionVector);

    public Point3D? IntersectionPointWith(LineBase other)
    {
        // (D) = this.DirectionVector, (E) = other.DirectionVector
        // P = this.FixedPoint, Q = other.FixedPoint
        // Solve: (OP) + x(D) = (OQ) + y(E) = (OR), in which R indicates the intersection point.
        // Solution: x(D) - y(E) = (OQ) - (OP) = (PQ).
        var pq = FixedPoint.VectorTo(other.FixedPoint);
        var result = Matrix<double>.Build.DenseOfColumns(new []
        {
            DirectionVector.ToEnumerable(),
            (-1 * other.DirectionVector).ToEnumerable()
        }).Solve(Vector<double>.Build.DenseOfEnumerable(pq.ToEnumerable()));
        
        // Check whether solution exists and construct result.
        if (result is not { Count: 2 }) return null;
        double x = result[0];
        var resultPoint = FixedPoint.MoveBy(x * DirectionVector);

        // Check whether the result is valid.
        return Contains(resultPoint) && other.Contains(resultPoint) ? resultPoint : null;
    }

    public Point3D? IntersectionPointWith(Plane3D plane) => plane.IntersectionPointWith(this);
    
    public double IncludedAngleTo(Line3D other) => Math.Acos(DirectionVector * other.DirectionVector);

    private bool Equals(LineBase other)
    {
        if (GetType() != other.GetType()) return false;
        if (this is Line3D) return DirectionVector.IsParallelTo(other.DirectionVector) && Contains(other.FixedPoint);
        if (this is HalfLine3D) return DirectionVector == other.DirectionVector && FixedPoint == other.FixedPoint;
        if (this is Segment3D thisSegment && other is Segment3D otherSegment)
            return thisSegment.StartPoint == otherSegment.StartPoint && thisSegment.EndPoint == otherSegment.EndPoint ||
                   thisSegment.StartPoint == otherSegment.EndPoint && thisSegment.EndPoint == otherSegment.StartPoint;
        return false;
    }

    public static bool operator ==(LineBase line1, LineBase line2) => line1.Equals(line2);
    public static bool operator !=(LineBase line1, LineBase line2) => !line1.Equals(line2);
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is LineBase other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(FixedPoint, DirectionVector);
}