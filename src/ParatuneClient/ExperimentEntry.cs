namespace ParatuneClient;

public class ExperimentEntry
{
    public Guid EntryId { get; set; }
    public int InitialPopulation { get; set; }
    public double CrossoverProbability { get; set; }
    public double MutationProbability { get; set; }
    public double ParentSelectionRatio { get; set; }
    public int Generation1 { get; set; }
    public int Generation2 { get; set; }
    public int Generation3 { get; set; }
    public int Generation4 { get; set; }
    public int Generation5 { get; set; }
    public Guid AssigneeId { get; set; }
    public Guid ModelId { get; set; }
}