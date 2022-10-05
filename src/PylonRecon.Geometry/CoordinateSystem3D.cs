namespace PylonRecon.Geometry;

public class CoordinateSystem3D
{
    private static CoordinateSystem3D? _standard;

    public static CoordinateSystem3D Standard =>
        _standard ??= new(Point3D.Origin, (1d, 0d, 0d), (0d, 1d, 0d), (0d, 0d, 1d));

    public CoordinateSystem3D(Point3D systemOrigin, Vector3D vectorOx, Vector3D vectorOy, Vector3D vectorOz)
    {
        SystemOrigin = systemOrigin;
        VectorOx = vectorOx.Normalize();
        VectorOy = vectorOy.Normalize();
        VectorOz = vectorOz.Normalize();
        if (VectorOz != (VectorOx ^ VectorOy))
            throw new ArithmeticException("Specified vectors cannot make a coordinate system.");
    }
    
    public Point3D SystemOrigin { get; }
    public Vector3D VectorOx { get; }
    public Vector3D VectorOy { get; }
    public Vector3D VectorOz { get; }
}