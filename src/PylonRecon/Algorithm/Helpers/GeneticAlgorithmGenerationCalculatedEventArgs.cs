namespace PylonRecon.Algorithm.Helpers;

public class GeneticAlgorithmGenerationCalculatedEventArgs<TTrait> : EventArgs
{
    public List<(TTrait, double)> CurrentPopulation { get; set; }
    public int Generation { get; set; }
}