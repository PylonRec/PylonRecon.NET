using PylonRecon.Algorithm;
using PylonRecon.Algorithm.Helpers;
using PylonRecon.Geometry;
using PylonRecon.Helpers;

namespace PylonRecon;

public class CentralAxisFinder
{
    private readonly PointCloud _pointCloud;
    public Point3D GeometricCenter { get; }

    public event EventHandler<EventArgs>? GenerationCalculated; 

    public CentralAxisFinder(PointCloud pointCloud)
    {
        _pointCloud = pointCloud;
        double xCenter = pointCloud.Locations.Average(static p => p.X);
        double yCenter = pointCloud.Locations.Average(static p => p.Y);
        double zCenter = pointCloud.Locations.Average(static p => p.Z);
        GeometricCenter = (xCenter, yCenter, zCenter);
    }

    public Line3D? FindCentralAxis(int maxGeneration, double crossoverProbability, double mutationProbability,
        double populationSelectionRatio, int initialPopulationScale)
    {
        GeneticAlgorithm<uint, (double, double)> primaryGeneticAlgorithm = new(new PrimarySymmetricPlaneGeneCodec(), PrimaryFitness, PrimaryGeneCrossover,
            PrimaryGeneMutation);
        primaryGeneticAlgorithm.GenerationCalculated += PrimaryGeneticAlgorithmOnGenerationCalculated;
        
        HashSet<(double, double)> initialPopulation = new();
        Random rand = new();
        while (initialPopulation.Count < initialPopulationScale)
        {
            double alpha = rand.NextDouble() * 2d * Math.PI;
            double beta = rand.NextDouble() * Math.PI - Math.PI / 2;
            initialPopulation.Add((alpha, beta));
        }

        var trait = primaryGeneticAlgorithm.Compute(initialPopulation, crossoverProbability, mutationProbability,
            populationSelectionRatio, maxGeneration);
        double dirX = Math.Cos(trait.Item1) * Math.Sin(trait.Item2);
        double dirY = Math.Sin(trait.Item1) * Math.Sin(trait.Item2);
        double dirZ = Math.Cos(trait.Item2);
        Plane3D symmetricPlane = new(GeometricCenter, new Vector3D(dirX, dirY, dirZ));
        primaryGeneticAlgorithm.GenerationCalculated -= PrimaryGeneticAlgorithmOnGenerationCalculated;

        var primaryPlaneXAxis = symmetricPlane.NormalVector.GetPerpendicularVectorSample().Normalize();
        var primaryPlaneYAxis = (symmetricPlane.NormalVector ^ primaryPlaneXAxis).Normalize();
        
        GeneticAlgorithm<uint, double> secondaryGeneticAlgorithm = new(new SecondarySymmetricPlaneGeneCodec(), 
            theta =>
            {
                Plane3D symmetricPlane = new(GeometricCenter, primaryPlaneXAxis * Math.Cos(theta) + primaryPlaneYAxis * Math.Sin(theta));
                return 1d / _pointCloud.Sum(p => Math.Pow(p.Location.DistanceTo(symmetricPlane), 2d));
            },
            (gene1, gene2, crossoverProbability) =>
            {
                uint child1 = gene1;
                uint child2 = gene2;
                Random rand = new();
                for (int i = 16; i < 32; i++)
                {
                    if (rand.NextDouble() < crossoverProbability)
                    {
                        child1 = child1.ModifyBit(i, gene2.GetBit(i));
                        child2 = child2.ModifyBit(i, gene1.GetBit(i));
                    }
                }
                return (child1, child2);
            },
            (gene, mutationProbability) =>
            {
                Random rand = new();
                for (int i = 16; i < 32; i++)
                {
                    if (rand.NextDouble() < mutationProbability)
                    {
                        gene = gene.ModifyBit(i, !gene.GetBit(i));
                    }
                }
                return gene;
            });
        secondaryGeneticAlgorithm.GenerationCalculated += SecondaryGeneticAlgorithmOnGenerationCalculated;
        
        HashSet<double> secondaryInitialPopulation = new();
        while (secondaryInitialPopulation.Count < initialPopulationScale)
        {
            secondaryInitialPopulation.Add(rand.NextDouble() * Math.PI);
        }

        var secondaryTrait = secondaryGeneticAlgorithm.Compute(secondaryInitialPopulation, crossoverProbability, mutationProbability,
            populationSelectionRatio, maxGeneration);
        Plane3D secondarySymmetricPlane = new(GeometricCenter,
            primaryPlaneXAxis * Math.Cos(secondaryTrait) + primaryPlaneYAxis * Math.Sin(secondaryTrait));

        return symmetricPlane.IntersectionLineWith(secondarySymmetricPlane);
    }

    private void PrimaryGeneticAlgorithmOnGenerationCalculated(object? sender, GeneticAlgorithmGenerationCalculatedEventArgs<(double, double)> e)
    {
        GenerationCalculated?.Invoke(this, e);
    }
    
    private void SecondaryGeneticAlgorithmOnGenerationCalculated(object? sender, GeneticAlgorithmGenerationCalculatedEventArgs<double> e)
    {
        GenerationCalculated?.Invoke(this, e);
    }

    private double PrimaryFitness((double Alpha, double Beta) trait)
    {
        double dirX = Math.Cos(trait.Alpha) * Math.Sin(trait.Beta);
        double dirY = Math.Sin(trait.Alpha) * Math.Sin(trait.Beta);
        double dirZ = Math.Cos(trait.Beta);
        Plane3D symmetricPlane = new(GeometricCenter, new Vector3D(dirX, dirY, dirZ));
        return 1d / _pointCloud.Sum(p => Math.Pow(p.Location.DistanceTo(symmetricPlane), 2d));
    }
    
    private (uint, uint) PrimaryGeneCrossover(uint gene1, uint gene2, double crossoverProbability)
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
    
    private uint PrimaryGeneMutation(uint gene, double mutationProbability)
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

    private class PrimarySymmetricPlaneGeneCodec : IValueConverter<(double Alpha, double Beta), uint>
    {
        public uint Convert((double Alpha, double Beta) from)
        {
            uint higher = (uint)Math.Floor(from.Alpha / 2d / Math.PI * 65536d);
            uint lower = (uint)Math.Floor((from.Beta + Math.PI / 2d) / Math.PI * 65536d);
            return (higher << 16) | lower;
        }

        public (double Alpha, double Beta) ConvertBack(uint backFrom)
        {
            uint higher = backFrom >> 16;
            uint lower = backFrom & 0x0000ffff;
            return (higher / 65536d * 2d * Math.PI, lower / 65536d * Math.PI - Math.PI / 2d);
        }
    }

    private class SecondarySymmetricPlaneGeneCodec : IValueConverter<double, uint>
    {
        public uint Convert(double from) => (uint) Math.Floor(from / Math.PI * 65536d);

        public double ConvertBack(uint backFrom) => backFrom / 65536d * Math.PI;
    }
}