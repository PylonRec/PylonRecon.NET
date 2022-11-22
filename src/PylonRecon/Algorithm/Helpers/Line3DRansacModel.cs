using PylonRecon.Geometry;

namespace PylonRecon.Algorithm.Helpers;

public class Line3DRansacModel : ISampleConsensusModel<Point3D, Line3D>
{
    public int MinimumSampleCount => 2;

    // public int a { get; set; }
    
    // private int a;
    // public int getA() { return a; }
    // public void setA(int value) { a = value; }
    public Line3D FitModelFromSample(IEnumerable<Point3D> input)
    {
        var sources = input.ToList();
        if (sources.Count != MinimumSampleCount) throw new Exception("Model misused.");
        return new(sources[0], sources[1]);
    }

    public double ModelError(Line3D model, Point3D source) => source.DistanceTo(model);
}