using PylonRecon.Algorithm.Helpers;

namespace PylonRecon.Algorithm;

public class RandomSampleConsensusAlgorithm<TSource, TModel> where TModel : class
{
    private readonly ISampleConsensusModel<TSource, TModel> _model;

    public RandomSampleConsensusAlgorithm(ISampleConsensusModel<TSource, TModel> model)
    {
        _model = model;
    }

    public (TModel, IEnumerable<TSource> inliers)? ComputeModel(IEnumerable<TSource> input, int maxIterations, double threshold,
        int assertionRequirement)

    {
        int iterations = 0;
        TModel? bestFit = null;
        double bestError = double.MaxValue;
        List<TSource> sources = input.ToList();
        List<TSource> inliers = new();

        while (iterations < maxIterations)
        {
            Random rand = new();
            HashSet<int> hypotheticalInlierIndices = new();
            while (hypotheticalInlierIndices.Count < _model.MinimumSampleCount)
            {
                hypotheticalInlierIndices.Add(rand.Next(sources.Count));
            }

            var hypotheticalInliers = hypotheticalInlierIndices.Select(i => sources[i]).ToList();
            var hypotheticalModel = _model.FitModelFromSample(hypotheticalInliers);

            List<TSource> computedInliers = new();
            foreach (var source in sources)
            {
                if (hypotheticalInliers.Contains(source)) continue;
                if (_model.ModelError(hypotheticalModel, source) < threshold)
                {
                    computedInliers.Add(source);
                }
            }

            if (computedInliers.Count > assertionRequirement)
            {
                var currentError = 1d / computedInliers.Sum(inlier => 1d / _model.ModelError(hypotheticalModel, inlier));
                if (currentError < bestError)
                {
                    bestFit = hypotheticalModel;
                    bestError = currentError;
                    inliers = computedInliers;
                }
            }

            iterations++;
        }

        if (bestFit is null) return null;
        return (bestFit, inliers);
    }
}