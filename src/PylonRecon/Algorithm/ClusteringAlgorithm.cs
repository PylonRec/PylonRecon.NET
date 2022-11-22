namespace PylonRecon.Algorithm;

public static class ClusteringAlgorithm<T>
{
    public static List<List<T>> Cluster(IEnumerable<T> source, Func<T, T, bool> clusteringFeatureFunc)
    {
        List<List<T>> result = new();
        foreach (var individual in source)
        {
            var cluster = result.FirstOrDefault(c => c.Any(i => clusteringFeatureFunc(individual, i)));
            if (cluster is not null) cluster.Add(individual);
            else result.Add(new() {individual});
        }
        return result;
    }
}