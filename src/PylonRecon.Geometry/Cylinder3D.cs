namespace PylonRecon.Geometry;

public sealed class Cylinder3D : IInterpolatable
{
    private readonly Point3D _topCenter;
    private readonly Point3D _bottomCenter;
    private readonly double _radius;

    public Cylinder3D(Point3D topCenter, Point3D bottomCenter, double radius)
    {
        _topCenter = topCenter;
        _bottomCenter = bottomCenter;
        _radius = radius;
    }

    public IEnumerable<Point3D> GetInterpolatedSample(double interpolationSpacing)
    {
        var verticalDirection = _topCenter.VectorTo(_bottomCenter).Normalize();
        var xAxis = verticalDirection.GetPerpendicularVectorSample().Normalize();
        var yAxis = (verticalDirection ^ xAxis).Normalize();

        var boundaryInside = (int)Math.Floor(_radius / interpolationSpacing);

        List<Vector3D> verticalAxisTranslation = new();
        for (int i = -1 * boundaryInside; i <= boundaryInside; i++)
        {
            for (int j = -1 * boundaryInside; j <= boundaryInside; j++)
            {
                if ((i * i + j * j) * interpolationSpacing * interpolationSpacing >= _radius * _radius) continue;
                verticalAxisTranslation.Add(xAxis * i + yAxis * j);
            }
        }
        
        List<double> circumferenceRotation = new();
        if (interpolationSpacing < Math.Sqrt(2d) * _radius)
        {
            double rotationUnit = 2d * Math.Asin(interpolationSpacing / 2d / _radius);
            for (double rot = 0d; rot < 2 * Math.PI; rot += rotationUnit)
            {
                circumferenceRotation.Add(rot);
            }
        }
        else
        {
            circumferenceRotation.AddRange(new[] {0d, Math.PI / 2d, Math.PI, Math.PI * 3d / 2d});
        }

        verticalAxisTranslation.AddRange(circumferenceRotation.Select(theta =>
            _radius * (Math.Cos(theta) * xAxis + Math.Sin(theta) * yAxis)));

        return verticalAxisTranslation.SelectMany(translation =>
            new Segment3D(_topCenter.MoveBy(translation), _bottomCenter.MoveBy(translation)).GetInterpolatedSample(
                interpolationSpacing));
    }
}