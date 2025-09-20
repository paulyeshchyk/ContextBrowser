using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFiller<TKey> : IContextInfoFiller<TKey>
    where TKey : notnull
{
    private readonly ITensorFactory<TKey> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IWordRoleClassifier _wordRoleClassifier;
    private readonly IEmptyDimensionClassifier _emptyDimensionClassifier;
    private readonly IFakeDimensionClassifier _FakeDimensionClassifier;

    public int Order { get; } = int.MinValue;

    public ContextInfoFiller(ITensorFactory<TKey> keyFactory, ITensorBuilder keyBuilder, IAppOptionsStore appOptionsStore)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _wordRoleClassifier = appOptionsStore.GetOptions<IWordRoleClassifier>();
        _emptyDimensionClassifier = appOptionsStore.GetOptions<IEmptyDimensionClassifier>();
        _FakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo, TKey> contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions)
    {
        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        foreach (var item in elements)
        {
            var verbs = item.Contexts.Where(c => _wordRoleClassifier.IsVerb(c, _FakeDimensionClassifier)).Distinct().ToList();
            var nouns = item.Contexts.Where(c => _wordRoleClassifier.IsNoun(c, _FakeDimensionClassifier)).Distinct().ToList();

            if (verbs.Any() && nouns.Any())
            {
                foreach (var row in verbs)
                {
                    foreach (var col in nouns)
                    {
                        var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { row, col }, _keyFactory.Create);
                        contextInfoData.Add(item, key);
                    }
                }
            }
            else if (verbs.Any())
            {
                foreach (var row in verbs)
                {
                    var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { row, _emptyDimensionClassifier.EmptyDomain }, _keyFactory.Create);
                    contextInfoData.Add(item, key);
                }
            }
            else if (nouns.Any() && includeUnclassified)
            {
                foreach (var col in nouns)
                {
                    var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { _emptyDimensionClassifier.EmptyAction, _emptyDimensionClassifier.EmptyDomain }, _keyFactory.Create);
                    contextInfoData.Add(item, key);
                }
            }
            else if (includeUnclassified)
            {
                var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { _emptyDimensionClassifier.EmptyAction, _emptyDimensionClassifier.EmptyDomain }, _keyFactory.Create);
                contextInfoData.Add(item, key);
            }
        }
    }
}
