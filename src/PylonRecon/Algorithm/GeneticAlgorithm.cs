using PylonRecon.Helpers;

namespace PylonRecon.Algorithm;

public class GeneticAlgorithm<TGene, TTrait> where TGene : struct
{
    private readonly IValueConverter<TTrait, TGene> _traitGeneConverter;
    private readonly Func<TTrait, double> _originalFeatureFunc;
    private readonly Func<List<double>, List<double>> _fitnessFunc;
    private readonly Func<TGene, TGene, double, (TGene, TGene)> _geneCrossoverFunc;
    private readonly Func<TGene, double, TGene> _geneMutationFunc;

    public GeneticAlgorithm(
        IValueConverter<TTrait, TGene> traitGeneConverter,
        Func<TTrait, double> originalFeatureFunc,
        Func<TGene, TGene, double, (TGene, TGene)> geneCrossoverFunc,
        Func<TGene, double, TGene> geneMutationFunc, 
        Func<List<double>, List<double>> fitnessFunc)
    {
        _traitGeneConverter = traitGeneConverter;
        _originalFeatureFunc = originalFeatureFunc;
        _geneCrossoverFunc = geneCrossoverFunc;
        _geneMutationFunc = geneMutationFunc;
        _fitnessFunc = fitnessFunc;
    }

    private List<TTrait> ComputeRound(
        List<TTrait> population,
        double crossoverProbability,
        double mutationProbability)
    {
        var originalFeature = population
            .Select(t => _originalFeatureFunc(t)).ToList();
        var fitness = _fitnessFunc(originalFeature);
        var originalFeatureMin = originalFeature.Min();
        int minIndex = originalFeature.IndexOf(originalFeatureMin);
        Console.WriteLine($"{originalFeatureMin} {population[minIndex]}");
        Random rand = new();

        // Selection
        // List<double> fitnessPrefixSum = new();
        // double fitnessSum = 0d;
        // for (int i = 0; i < population.Count; i++)
        // {
        //     fitnessSum += fitness[i];
        //     fitnessPrefixSum.Add(fitnessSum);
        // }
        List<TGene> selectedParents = new();
        while (selectedParents.Count < population.Count / 2)
        {
            // int index = fitnessPrefixSum.FindIndex(s => rand.NextDouble() * fitnessSum < s);
            // selectedParents.Add(_traitGeneConverter.Convert(population[index]));
            int first = rand.Next(population.Count);
            int second = rand.Next(population.Count);
            if (first == second) continue;
            selectedParents.Add(_traitGeneConverter.Convert(fitness[first] >= fitness[second]
                ? population[first]
                : population[second]));
        }
        
        // Crossover
        List<TGene> offsprings = new(); 
        while (offsprings.Count + selectedParents.Count < population.Count)
        {
            var children = _geneCrossoverFunc(selectedParents[rand.Next(selectedParents.Count)],
                selectedParents[rand.Next(selectedParents.Count)], crossoverProbability);
            offsprings.Add(children.Item1);
        }
        
        // Mutation
        for (var i = 0; i < offsprings.Count; i++)
        {
            offsprings[i] = _geneMutationFunc(offsprings[i], mutationProbability);
        }
        
        List<TGene> result = new();
        result.AddRange(selectedParents);
        result.AddRange(offsprings);
        return result.Select(g => _traitGeneConverter.ConvertBack(g)).ToList();
    }

    public TTrait? Compute(
        IEnumerable<TTrait> initialPopulation,
        double crossoverProbability,
        double mutationProbability,
        int maxGenerations)
    {
        List<TTrait> currentPopulation = initialPopulation.ToList();
        for (int i = 0; i < maxGenerations; i++)
        {
            currentPopulation = ComputeRound(currentPopulation, crossoverProbability, mutationProbability);
        }
        return currentPopulation.MaxBy(t => _originalFeatureFunc(t));
    }
}