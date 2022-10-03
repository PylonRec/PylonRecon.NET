namespace PylonRecon.Geometry;

public class Triangle3D : IInterpolatable
{
    public Point3D PointA { get; }
    public Point3D PointB { get; }
    public Point3D PointC { get; }

    public double AbLength => PointA.DistanceTo(PointB);
    public double AcLength => PointA.DistanceTo(PointC);
    public double BcLength => PointB.DistanceTo(PointC);

    private Plane3D? _plane;
    public Plane3D Plane => _plane ??= new(PointA, PointB, PointC);
    
    public Triangle3D(Point3D pointA, Point3D pointB, Point3D pointC)
    {
        Line3D ab = new Line3D(pointA, pointB);
        if (ab.Contains(pointC)) throw new ArithmeticException("Three points are on the same line.");
        PointA = pointA;
        PointB = pointB;
        PointC = pointC;
    }

    public IEnumerable<Point3D> GetInterpolatedSample(double interpolationSpacing)
    {
        List<Point3D> result = new();
        // First generate AB-line translation direction.
        // Let D on line AB making AB perpendicular to CD. We have
        // (AD) = k (AB), (CD) * (AB) = 0, which is to say, ( (CA) + k (AB) ) * (AB) = 0
        // Solution: k (AB) * (AB) = - (CA) * (AB).
        // k = - ( (CA) * (AB) ) / ( (AB) * (AB) ).
        var ca = PointC.VectorTo(PointA);
        var ab = PointA.VectorTo(PointB);
        var k = -1 * (ca * ab) / (ab * ab);
        var d = PointA.MoveBy(k * ab);
        // Triangle's height on side AB, which is also the translation distance from AB to C.
        var height = d.DistanceTo(PointC);
        // Translation direction base vector.
        var dcNormalized = d.VectorTo(PointC).Normalize();

        Line3D acLine = new(PointA, PointC);
        Line3D bcLine = new(PointB, PointC);
        
        for (double translationDistance = 0d; translationDistance < height; translationDistance += interpolationSpacing)
        {
            Line3D translatedLine = new(PointA.MoveBy(translationDistance * dcNormalized), ab);
            var a = translatedLine.IntersectionPointWith(acLine);
            var b = translatedLine.IntersectionPointWith(bcLine);
            if (a is null || b is null) continue;
            try
            {
                Segment3D translatedSegment = new(a, b);
                result.AddRange(translatedSegment.GetInterpolatedSample(interpolationSpacing));
            }
            catch
            {
                // ignored
            }
        }
        
        result.Add(PointC);
        return result;
    }
}