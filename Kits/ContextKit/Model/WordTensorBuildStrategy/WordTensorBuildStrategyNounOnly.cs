using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model.WordTensorBuildStrategy;

public class WordTensorBuildStrategyNounOnly<TTensor> : IWordTensorBuildStrategy<TTensor>
    where TTensor : notnull
{
    private readonly ITensorBuilder _tensorBuilder;
    private readonly ITensorFactory<TTensor> _tensorFactory;
    private readonly IEmptyDimensionClassifier _emptyDimensionClassifier;

    public int Priority => 2;

    public WordTensorBuildStrategyNounOnly(ITensorBuilder tensorBuilder, IAppOptionsStore appOptionsStore, ITensorFactory<TTensor> tensorFactory)
    {
        _tensorBuilder = tensorBuilder;
        _tensorFactory = tensorFactory;
        _emptyDimensionClassifier = appOptionsStore.GetOptions<IEmptyDimensionClassifier>();
    }

    public IEnumerable<TTensor> BuildTensors(ContextElementGroups contextElementGroups)
    {
        return contextElementGroups.Nouns.Select(noun => _tensorBuilder.BuildTensor(TensorPermutationType.Standard, [_emptyDimensionClassifier.EmptyAction, noun], _tensorFactory.Create));
    }

    public bool CanHandle(ContextElementGroups contextElementGroups, ExportMatrixOptions matrixOptions)
    {
        bool includeUnclassified = matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;
        return contextElementGroups.Nouns.Any() && !contextElementGroups.Verbs.Any() && includeUnclassified;
    }
}
