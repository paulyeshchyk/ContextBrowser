using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model.WordTensorBuildStrategy;

public class WordTensorBuildStrategyVerbNoun<TTensor> : IWordTensorBuildStrategy<TTensor>
    where TTensor : notnull
{
    private readonly ITensorBuilder _tensorBuilder;
    private readonly ITensorFactory<TTensor> _tensorFactory;

    public int Priority => 1;

    public WordTensorBuildStrategyVerbNoun(ITensorBuilder tensorBuilder, ITensorFactory<TTensor> tensorFactory)
    {
        _tensorBuilder = tensorBuilder;
        _tensorFactory = tensorFactory;
    }

    public IEnumerable<TTensor> BuildTensors(ContextElementGroups contextElementGroups)
    {
        return contextElementGroups.Verbs.SelectMany(row => contextElementGroups.Nouns.Select(col =>
            _tensorBuilder.BuildTensor(TensorPermutationType.Standard, new object[] { row, col }, _tensorFactory.Create)));
    }

    public bool CanHandle(ContextElementGroups contextElementGroups, ExportMatrixOptions matrixOptions)
    {
        return contextElementGroups.Verbs.Any() && contextElementGroups.Nouns.Any();
    }
}
