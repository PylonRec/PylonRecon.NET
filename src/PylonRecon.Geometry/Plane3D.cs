using PylonRecon.Geometry.Helpers;

namespace PylonRecon.Geometry;

public sealed class Plane3D
{
    internal Point3D CenterPoint { get; }
    public Vector3D NormalVector { get; }
    
    public Plane3D(Point3D centerPoint, Vector3D normalVector)
    {
        if (normalVector.Length.IsZero())
            throw new ArithmeticException("A plane must be determined with a fixed point and its normal vector.");
        CenterPoint = centerPoint;
        NormalVector = normalVector.Normalize();
    }

    public Plane3D(Point3D pointA, Point3D pointB, Point3D pointC)
    {
        if (pointA == pointB || pointA == pointC || pointB == pointC)
            throw new ArithmeticException("A plane must be determined with 3 different points.");
        CenterPoint = pointA;
        NormalVector = (pointA.VectorTo(pointB) ^ pointA.VectorTo(pointC)).Normalize();
    }

    public Plane3D(Line3D lineA, Line3D lineB)
    {
        if (lineA.CorrespondingLine == lineB.CorrespondingLine)
            throw new ArithmeticException("The lines provided can't determine a plane.");
        if (lineA.IsParallelTo(lineB))
        {
            CenterPoint = lineA.FixedPoint;
            NormalVector = (lineA.DirectionVector ^ lineA.FixedPoint.VectorTo(lineB.FixedPoint)).Normalize();
            return;
        }
        if (lineA.IntersectionPointWith(lineB) is { } intersectionPoint)
        {
            CenterPoint = intersectionPoint;
            NormalVector = (lineA.DirectionVector ^ lineB.DirectionVector).Normalize();
            return;
        }
        throw new ArithmeticException("Only two parallel or intersecting lines can determine a plane.");
    }

    #region equality comparing operators

    public static bool operator ==(Plane3D plane1, Plane3D plane2) => plane1.Equals(plane2);
    public static bool operator !=(Plane3D plane1, Plane3D plane2) => !plane1.Equals(plane2);

    private bool Equals(Plane3D other) => NormalVector.IsParallelTo(other.NormalVector) && Contains(other.CenterPoint);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Plane3D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(CenterPoint, NormalVector);

    #endregion
    
    public bool Contains(Point3D point) => (CenterPoint.VectorTo(point) * NormalVector).IsZero();

    public bool Contains(LineBase3D line) =>
        NormalVector.IsPerpendicularTo(line.DirectionVector) && Contains(line.FixedPoint);

    public bool IsParallelTo(LineBase3D line) =>
        NormalVector.IsPerpendicularTo(line.DirectionVector) && !Contains(line.FixedPoint);
    
    public bool IsParallelTo(Plane3D other) =>
        NormalVector.IsParallelTo(other.NormalVector) && !Contains(other.CenterPoint);

    public bool IsPerpendicularTo(LineBase3D line) => NormalVector.IsParallelTo(line.DirectionVector);

    public bool IsPerpendicularTo(Plane3D other) => NormalVector.IsPerpendicularTo(other.NormalVector);

    public Point3D? IntersectionPointWith(LineBase3D line)
    {
        // C = this.CenterPoint, F = line.FixedPoint
        // (N) = this.NormalVector, (D) = line.DirectionVector
        // Solve: ( (OF) + x(D) - (OC) ) · (N) = 0, in which
        // the intersection point P satisfies (OP) = (OF) + x(D).
        // Solution: ( (CF) + x(D) ) · (N) = 0
        // (CF) · (N) = -x(D) · (N)
        // x = - ( (CF) · (N) ) / ( (D) · (N) ), 
        // in which when (D) is perpendicular to (N)
        // ( the plane and the line are parallel or the line lies in the plane )
        // the solution does not exist.
        var cf = CenterPoint.VectorTo(line.FixedPoint);
        
        // Check whether the solution exists.
        double dn = line.DirectionVector * NormalVector;
        if (dn.IsZero()) return null;
        
        // Solve x and construct result.
        double x = -1 * (cf * NormalVector) / dn;
        var resultPoint = line.FixedPoint.MoveBy(x * line.DirectionVector);
        
        // Check whether the result is valid.
        return line.Contains(resultPoint) ? resultPoint : null;
    }

    public Line3D? IntersectionLineWith(Plane3D other)
    {
        // C = this.CenterPoint, D = other.CenterPoint
        // M = this.NormalVector, N = other.NormalVector
        // Solve: (E) · (M) = (E) · (N) = 0, in which (E) is the direction vector of the intersection line.
        // Solution: let (E) = (M) × (N).
        var direction = NormalVector ^ other.NormalVector;

        // Check whether the planes are parallel or the same.
        if (direction == Vector3D.Zero) return null;

        // Locate P on the intersection line where DP is perpendicular to (E).
        // Additionally we already have (DP) perpendicular to normal vector N,
        // thus we can define (DP) = k ((N) × (E)) (k ∈ R), in which we define (F) = (N) × (E)
        // We only need to solve k to calculate coordinate of P.
        // Obviously P is on both planes so (CP) · (M) = 0.
        // With definitions and conditions above, we derive that
        // ((CD) + k (F)) · (M) = 0.
        // Solution: k ( (F) · (M) ) = - (CD) · (M).
        // k = - ( (CD) · (M) ) / ( (F) · (M) ).
        var f = other.NormalVector ^ direction;
        double k = -1 * (CenterPoint.VectorTo(other.CenterPoint) * NormalVector) / (f * NormalVector);
        var fixedPoint = other.CenterPoint.MoveBy(k * f);

        return new(fixedPoint, direction);
    }

    public double IncludedAngleWith(Line3D line) => Math.Asin(NormalVector * line.DirectionVector);

    public double IncludedAngleWith(Plane3D other) => Math.Acos(NormalVector * other.NormalVector);
    
    public double DistanceTo(Point3D point) => Math.Abs(CenterPoint.VectorTo(point) * NormalVector);

    public double? DistanceTo(Line3D line) => IsParallelTo(line) ? DistanceTo(line.FixedPoint) : null;

    public double? DistanceTo(Plane3D other) => IsParallelTo(other) ? DistanceTo(other.CenterPoint) : null;

    public double RelativeAltitudeOf(Point3D point) => CenterPoint.VectorTo(point) * NormalVector;
}