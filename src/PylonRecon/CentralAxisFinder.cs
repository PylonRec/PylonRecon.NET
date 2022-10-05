using PylonRecon.Algorithm;
using PylonRecon.Geometry;
using PylonRecon.Helpers;

namespace PylonRecon;

public class CentralAxisFinder
{
    private readonly GeneticAlgorithm<uint, (double, double)> _geneticAlgorithm;
    private readonly PointCloud _pointCloud;
    private readonly Point3D _geometricCenter;

    public CentralAxisFinder(PointCloud pointCloud)
    {
        _geneticAlgorithm = new(new CentralAxisDirectionGeneCodec(), OriginalFeature, GeneCrossover, GeneMutation, Fitness);
        _pointCloud = pointCloud;
        double xCenter = pointCloud.Locations.Average(static p => p.X);
        double yCenter = pointCloud.Locations.Average(static p => p.Y);
        double zCenter = pointCloud.Locations.Average(static p => p.Z);
        _geometricCenter = (xCenter, yCenter, zCenter);
    }

    public Line3D FindCentralAxis(int maxGeneration, double crossoverProbability, double mutationProbability)
    {
        HashSet<(double, double)> initialPopulation = new();
        Random rand = new();
        while (initialPopulation.Count < 16)
        {
            double alpha = rand.NextDouble() * 2d * Math.PI;
            double beta = rand.NextDouble() * Math.PI;
            initialPopulation.Add((alpha, beta));
        }
        var trait = _geneticAlgorithm.Compute(initialPopulation, crossoverProbability, mutationProbability, maxGeneration);
        double dirX = Math.Cos(trait.Item1) * Math.Cos(trait.Item2);
        double dirY = Math.Sin(trait.Item1) * Math.Cos(trait.Item2);
        double dirZ = Math.Sin(trait.Item2);
        return new(_geometricCenter, new Vector3D(dirX, dirY, dirZ));
    }
    
    private double OriginalFeature((double Alpha, double Beta) trait)
    {
        double dirX = Math.Cos(trait.Alpha) * Math.Cos(trait.Beta);
        double dirY = Math.Sin(trait.Alpha) * Math.Cos(trait.Beta);
        double dirZ = Math.Sin(trait.Beta);
        Line3D centralAxis = new(_geometricCenter, new Vector3D(dirX, dirY, dirZ));
        return _pointCloud.Sum(p => p.Location.DistanceTo(centralAxis));
    }
    
    private List<double> Fitness(List<double> originalFeature)
    {
        double max = originalFeature.Max();
        double min = originalFeature.Min();
        double fix = (max - min) / originalFeature.Count() / originalFeature.Count();
        return originalFeature.Select(d => max - d + fix).ToList();
    }
    
    private (uint, uint) GeneCrossover(uint gene1, uint gene2, double crossoverProbability)
    {
        uint child1 = gene1;
        uint child2 = gene2;
        Random rand = new();
        for (int i = 0; i < 32; i++)
        {
            if (rand.NextDouble() < crossoverProbability)
            {
                child1 = child1.ModifyBit(i, gene2.GetBit(i));
                child2 = child2.ModifyBit(i, gene1.GetBit(i));
            }
        }
        return (child1, child2);
    }
    
    private uint GeneMutation(uint gene, double mutationProbability)
    {
        Random rand = new();
        for (int i = 0; i < 32; i++)
        {
            if (rand.NextDouble() < mutationProbability)
            {
                gene = gene.ModifyBit(i, !gene.GetBit(i));
            }
        }
        return gene;
    }

    private class CentralAxisDirectionGeneCodec : IValueConverter<(double Alpha, double Beta), uint>
    {
        public uint Convert((double Alpha, double Beta) from)
        {
            uint higher = (uint)Math.Floor(from.Alpha / 2d / Math.PI * 65536d);
            uint lower = (uint)Math.Floor(from.Beta / Math.PI * 65536d);
            return (higher << 16) | lower;
        }

        public (double Alpha, double Beta) ConvertBack(uint backFrom)
        {
            uint higher = backFrom >> 16;
            uint lower = backFrom & 0x0000ffff;
            return (higher / 65536d * 2d * Math.PI, lower / 65536d * Math.PI);
        }
    }
}