namespace PylonRecon.Geometry.Helpers;

public static class MathHelpers
{
    public static bool IsEquivalentTo(this double a, double b) => Math.Abs(a - b) < 1e-5;

    public static bool IsZero(this double a) => Math.Abs(a) < 1e-5;

    public static Vector3D GetPerpendicularVector(Vector3D vector) =>
        vector switch
        {
            {X: 0} => (1, 0, 0),
            {Y: 0} => (0, 1, 0),
            {Z: 0} => (0, 0, 1),
            _ => (1, 1, -1 * (vector.X + vector.Y) / vector.Z)
        };
}