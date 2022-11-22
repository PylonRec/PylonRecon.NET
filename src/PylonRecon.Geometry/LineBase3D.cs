using MathNet.Numerics.LinearAlgebra;
using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public abstract class LineBase3D
{
    /// <summary>
    /// Direction vector of the object.
    /// </summary>
    public Vector3D DirectionVector { get; }
    
    /// <summary>
    /// For any <see cref="LineBase3D"/> object, returns the line that contains this object.
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
    internal LineBase3D(Point3D fixedPoint, Vector3D directionVector, double? minPosition = null,
        double? maxPosition = null)
    {
        if (directionVector.Length.IsZero())
            throw new ArithmeticException("Lines must be constructed with a given direction vector.");
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

    public bool IsParallelTo(LineBase3D other) => DirectionVector.IsParallelTo(other.DirectionVector);

    public bool IsPerpendicularTo(LineBase3D other) => DirectionVector.IsPerpendicularTo(other.DirectionVector);

    public Point3D? IntersectionPointWith(LineBase3D other)
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
    
    public double IncludedAngleWith(LineBase3D other) => Math.Acos(Math.Abs(DirectionVector * other.DirectionVector));

    public Plane3D SamplePerpendicularPlane() => new(FixedPoint, DirectionVector);
    
    private bool Equals(LineBase3D other)
    {
        if (GetType() != other.GetType()) return false;
        return this switch
        {
            Line3D => DirectionVector.IsParallelTo(other.DirectionVector) && Contains(other.FixedPoint),
            HalfLine3D => DirectionVector == other.DirectionVector && FixedPoint == other.FixedPoint,
            Segment3D thisSegment when other is Segment3D otherSegment =>
                thisSegment.StartPoint == otherSegment.StartPoint && thisSegment.EndPoint == otherSegment.EndPoint ||
                thisSegment.StartPoint == otherSegment.EndPoint && thisSegment.EndPoint == otherSegment.StartPoint,
            _ => false
        };
    }

    public static bool operator ==(LineBase3D line1, LineBase3D line2) => line1.Equals(line2);
    public static bool operator !=(LineBase3D line1, LineBase3D line2) => !line1.Equals(line2);
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is LineBase3D other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(FixedPoint, DirectionVector);
}