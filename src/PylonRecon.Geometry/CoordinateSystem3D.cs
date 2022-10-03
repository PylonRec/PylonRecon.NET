namespace PylonRecon.Geometry;

public class CoordinateSystem3D
{
    private static CoordinateSystem3D? _standard;
    public static CoordinateSystem3D Standard => _standard ??= new()
    {
        SystemOrigin = Point3D.Origin,
        VectorOX = (1, 0, 0),
        VectorOY = (0, 1, 0),
        VectorOZ = (0, 0, 1)
    };
    
    public Point3D SystemOrigin { get; set; }
    public Vector3D VectorOX { get; set; }
    public Vector3D VectorOY { get; set; }
    public Vector3D VectorOZ { get; set; }
}