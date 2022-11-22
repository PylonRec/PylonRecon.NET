using MathNet.Numerics.LinearAlgebra;
using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public abstract class LineBase2D
{
    public Vector2D DirectionVector { get; }

    public abstract Line2D CorrespondingLine { get; }

    internal readonly Point2D FixedPoint;

    private readonly double? _minPosition, _maxPosition;

    internal LineBase2D(Point2D fixedPoint, Vector2D directionVector, double? minPosition = null,
        double? maxPosition = null)
    {
        if (directionVector.Length.IsZero())
            throw new ArithmeticException("Lines must be constructed with a given direction vector.");
        DirectionVector = directionVector.Normalize();
        FixedPoint = fixedPoint;
        _minPosition = minPosition;
        _maxPosition = maxPosition;
    }

    public bool Contains(Point2D point)
    {
        var relativeVector = FixedPoint.VectorTo(point);
        if (!DirectionVector.IsParallelTo(relativeVector)) return false;
        var relativePosition = relativeVector.Length;
        return (_minPosition is null || relativePosition >= _minPosition) &&
               (_maxPosition is null || relativePosition <= _maxPosition);
    }

    public bool IsParallelTo(LineBase2D other) => DirectionVector.IsParallelTo(other.DirectionVector);

    public bool IsPerpendicularTo(LineBase2D other) => DirectionVector.IsPerpendicularTo(other.DirectionVector);

    public Point2D? IntersectionPointWith(LineBase2D other)
    {
        var pq = FixedPoint.VectorTo(other.FixedPoint);
        var result = Matrix<double>.Build.DenseOfColumns(new[]
        {
            DirectionVector.ToEnumerable(),
            (-1 * other.DirectionVector).ToEnumerable()
        }).Solve(Vector<double>.Build.DenseOfEnumerable(pq.ToEnumerable()));

        if (result is not {Count: 2}) return null;
        double x = result[0];
        var resultPoint = FixedPoint.MoveBy(x * DirectionVector);

        return Contains(resultPoint) && other.Contains(resultPoint) ? resultPoint : null;
    }

    public double IncludedAngleTo(LineBase2D other) => Math.Acos(Math.Abs(DirectionVector * other.DirectionVector));

    private bool Equals(LineBase2D other)
    {
        if (GetType() != other.GetType()) return false;
        return this switch
        {
            Line2D => DirectionVector.IsParallelTo(other.DirectionVector) && Contains(other.FixedPoint),
            HalfLine2D => DirectionVector == other.DirectionVector && FixedPoint == other.FixedPoint,
            Segment2D thisSegment when other is Segment2D otherSegment =>
                thisSegment.StartPoint == otherSegment.StartPoint && thisSegment.EndPoint == otherSegment.EndPoint ||
                thisSegment.StartPoint == otherSegment.EndPoint && thisSegment.EndPoint == otherSegment.StartPoint,
            _ => false
        };
    }

    public static bool operator ==(LineBase2D line1, LineBase2D line2) => line1.Equals(line2);
    public static bool operator !=(LineBase2D line1, LineBase2D line2) => !line1.Equals(line2);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is LineBase2D other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(FixedPoint, DirectionVector);
}