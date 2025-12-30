using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model.WordTensorBuildStrategy;

public class WordTensorBuildStrategyUnclassified<TTensor> : IWordTensorBuildStrategy<TTensor>
    where TTensor : notnull
{
    private readonly ITensorBuilder _tensorBuilder;
    private readonly ITensorFactory<TTensor> _tensorFactory;
    private readonly IEmptyDimensionClassifier _emptyDimensionClassifier;

    public int Priority => 3;

    public WordTensorBuildStrategyUnclassified(ITensorBuilder tensorBuilder, IAppOptionsStore appOptionsStore, ITensorFactory<TTensor> tensorFactory)
    {
        _tensorBuilder = tensorBuilder;
        _tensorFactory = tensorFactory;
        _emptyDimensionClassifier = appOptionsStore.GetOptions<IEmptyDimensionClassifier>();
    }

    public IEnumerable<TTensor> BuildTensors(ContextElementGroups contextElementGroups)
    {
        yield return _tensorBuilder.BuildTensor(TensorPermutationType.Standard,
            new object[] { _emptyDimensionClassifier.EmptyAction, _emptyDimensionClassifier.EmptyDomain },
            _tensorFactory.Create);
    }

    public bool CanHandle(ContextElementGroups contextElementGroups, ExportMatrixOptions matrixOptions)
    {
        bool includeUnclassified = matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;
        return includeUnclassified;
    }
}
