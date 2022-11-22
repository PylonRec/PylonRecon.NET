using PylonRecon.Geometry;

namespace PylonRecon.Algorithm.Helpers;

public class Plane3DRansacModel : ISampleConsensusModel<Point3D, Plane3D>
{
    public int MinimumSampleCount => 3;
    public Plane3D FitModelFromSample(IEnumerable<Point3D> input)
    {
        var sources = input.ToList();
        if (sources.Count != MinimumSampleCount) throw new Exception("Model misused.");
        return new(sources[0], sources[1], sources[2]);
    }

    public double ModelError(Plane3D model, Point3D source)
    {
        return source.DistanceTo(model);
    }
}