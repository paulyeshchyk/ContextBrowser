using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFillerEmptyData<TTensor> : IContextInfoFiller<TTensor>
    where TTensor : notnull
{
    private readonly ITensorFactory<TTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IContextClassifier<ContextInfo> _wordRoleClassifier;
    private readonly IEmptyDimensionClassifier _emptyDimensionClassifier;
    private readonly IFakeDimensionClassifier _FakeDimensionClassifier;

    public ContextInfoFillerEmptyData(ITensorFactory<TTensor> keyFactory, ITensorBuilder keyBuilder, IAppOptionsStore appOptionsStore)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _wordRoleClassifier = appOptionsStore.GetOptions<IContextClassifier<ContextInfo>>();
        _emptyDimensionClassifier = appOptionsStore.GetOptions<IEmptyDimensionClassifier>();
        _FakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    public int Order { get; } = int.MaxValue;

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo, TTensor> contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions)
    {
        if (!_matrixOptions.IncludeAllStandardActions)
            return;

        var allVerbs = _wordRoleClassifier.GetCombinedVerbs(elements.SelectMany(e => e.Contexts).Where(c => _wordRoleClassifier.IsVerb(c, _FakeDimensionClassifier)).Distinct().ToList()).ToList();
        var allNouns = elements.SelectMany(e => e.Contexts).Where(c => _wordRoleClassifier.IsNoun(c, _FakeDimensionClassifier)).Distinct().ToList();

        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        var nounsToUse = includeUnclassified
            ? allNouns.Append(_emptyDimensionClassifier.EmptyDomain)
            : allNouns;

        foreach (var row in allVerbs)
        {
            foreach (var col in nounsToUse)
            {
                var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { row, col }, _keyFactory.Create);
                contextInfoData.Add(null, key);
            }
        }

        if (includeUnclassified)
        {
            foreach (var noun in allNouns)
            {
                var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { _emptyDimensionClassifier.EmptyAction, noun }, _keyFactory.Create);
                contextInfoData.Add(null, key);
            }

            var emptyKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { _emptyDimensionClassifier.EmptyAction, _emptyDimensionClassifier.EmptyDomain }, _keyFactory.Create);
            contextInfoData.Add(null, emptyKey);
        }
    }
}