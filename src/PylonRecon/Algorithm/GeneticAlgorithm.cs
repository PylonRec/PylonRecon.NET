using PylonRecon.Algorithm.Helpers;
using PylonRecon.Helpers;

namespace PylonRecon.Algorithm;

public class GeneticAlgorithm<TGene, TTrait> where TGene : struct
{
    private readonly IValueConverter<TTrait, TGene> _traitGeneConverter;
    private readonly Func<TTrait, double> _fitnessFunc;
    private readonly Func<TGene, TGene, double, (TGene, TGene)> _geneCrossoverFunc;
    private readonly Func<TGene, double, TGene> _geneMutationFunc;

    public event EventHandler<GeneticAlgorithmGenerationCalculatedEventArgs<TTrait>>? GenerationCalculated;

    public GeneticAlgorithm(
        IValueConverter<TTrait, TGene> traitGeneConverter,
        Func<TTrait, double> fitnessFunc,
        Func<TGene, TGene, double, (TGene, TGene)> geneCrossoverFunc,
        Func<TGene, double, TGene> geneMutationFunc)
    {
        _traitGeneConverter = traitGeneConverter;
        _geneCrossoverFunc = geneCrossoverFunc;
        _geneMutationFunc = geneMutationFunc;
        _fitnessFunc = fitnessFunc;
    }

    private List<(TTrait Trait, double Fitness)> ComputeRound(
        List<(TTrait Trait, double Fitness)> population,
        double crossoverProbability,
        double mutationProbability,
        double populationSelectionRatio)
    {
        Random rand = new();
        
        // Selection
        List<(TGene Gene, double Fitness)> selectedParents = new();
        while (selectedParents.Count < populationSelectionRatio * population.Count)
        {
            int first = rand.Next(population.Count);
            int second = rand.Next(population.Count);
            if (first == second) continue;
            int selectedIndex = population[first].Fitness >= population[second].Fitness ? first : second;
            selectedParents.Add((_traitGeneConverter.Convert(population[selectedIndex].Trait), population[selectedIndex].Fitness));
        }
        
        // Crossover
        List<(TGene Gene, double Fitness)> offsprings = new();
        while (offsprings.Count + selectedParents.Count < population.Count)
        {
            int parent1 = rand.Next(selectedParents.Count), parent2 = rand.Next(selectedParents.Count);
            var children = _geneCrossoverFunc(selectedParents[parent1].Gene, selectedParents[parent2].Gene, crossoverProbability);
            var childToAdd = _traitGeneConverter.ConvertBack(children.Item1);
            var childFitness = _fitnessFunc(childToAdd);
            // if (childFitness < fitness[parent1] && childFitness < fitness[parent2]) continue;
            offsprings.Add((children.Item1, childFitness));
        }
        
        // Mutation
        for (var i = 0; i < offsprings.Count; i++)
        {
            var modifiedGene = _geneMutationFunc(offsprings[i].Gene, mutationProbability);
            offsprings[i] = (modifiedGene, _fitnessFunc(_traitGeneConverter.ConvertBack(modifiedGene)));
        }

        List<(TGene Gene, double Fitness)> newGeneration = new();
        newGeneration.AddRange(selectedParents);
        newGeneration.AddRange(offsprings);
        return (newGeneration.Select(g => (_traitGeneConverter.ConvertBack(g.Gene), g.Fitness)).ToList());
    }

    public TTrait Compute(
        IEnumerable<TTrait> initialPopulation,
        double crossoverProbability,
        double mutationProbability,
        double populationSelectionRatio,
        int maxGenerations)
    {
        List<(TTrait Trait, double Fitness)> currentPopulation = initialPopulation.Select(t => (t, _fitnessFunc(t))).ToList();
        for (int i = 0; i < maxGenerations; i++)
        {
            currentPopulation = ComputeRound(currentPopulation, crossoverProbability,
                mutationProbability, populationSelectionRatio);
            GenerationCalculated?.Invoke(this, new GeneticAlgorithmGenerationCalculatedEventArgs<TTrait>
            {
                CurrentPopulation = currentPopulation,
                Generation = i + 1
            });
        }
        return currentPopulation.MaxBy(t => t.Fitness).Trait;
    }

    public TTrait InfiniteCompute(
        IEnumerable<TTrait> initialPopulation,
        double crossoverProbability,
        double mutationProbability,
        double populationSelectionRatio,
        Predicate<TTrait> endingPredicate)
    {
        List<(TTrait Trait, double Fitness)> currentPopulation = initialPopulation.Select(t => (t, _fitnessFunc(t))).ToList();
        int generation = 1;
        while (true)
        {
            
            currentPopulation = ComputeRound(currentPopulation, crossoverProbability,
                mutationProbability, populationSelectionRatio);
            GenerationCalculated?.Invoke(this, new GeneticAlgorithmGenerationCalculatedEventArgs<TTrait>
            {
                CurrentPopulation = currentPopulation,
                Generation = generation
            });
            generation++;
            var optimumIndividual = currentPopulation.MaxBy(p => p.Fitness).Trait;
            if (endingPredicate(optimumIndividual) || generation > 200)
            {
                return optimumIndividual;
            }
        }
    }
}