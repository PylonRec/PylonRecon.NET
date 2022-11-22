namespace PylonRecon.Algorithm.Helpers;

public interface ISampleConsensusModel<TSource, TModel> where TModel : class
{
    int MinimumSampleCount { get; }

    TModel FitModelFromSample(IEnumerable<TSource> input);

    double ModelError(TModel model, TSource source);
}