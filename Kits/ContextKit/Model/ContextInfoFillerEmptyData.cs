using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFillerEmptyData<TKey> : IContextInfoFiller<TKey>
    where TKey : notnull
{
    private readonly ITensorFactory<TKey> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;

    public ContextInfoFillerEmptyData(ITensorFactory<TKey> keyFactory, ITensorBuilder keyBuilder)
    {
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
    }

    public int Order { get; } = int.MaxValue;

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo, TKey> contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions, IDomainPerActionContextTensorClassifier contextClassifier)
    {
        if (!_matrixOptions.IncludeAllStandardActions)
            return;

        var allVerbs = contextClassifier.GetCombinedVerbs(elements.SelectMany(e => e.Contexts).Where(contextClassifier.IsVerb).Distinct().ToList()).ToList();
        var allNouns = elements.SelectMany(e => e.Contexts).Where(contextClassifier.IsNoun).Distinct().ToList();

        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        var nounsToUse = includeUnclassified
            ? allNouns.Append(contextClassifier.EmptyDomain)
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
                var key = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { contextClassifier.EmptyAction, noun }, _keyFactory.Create);
                contextInfoData.Add(null, key);
            }

            var emptyKey = _keyBuilder.BuildTensor(TensorPermutationType.Standard, new[] { contextClassifier.EmptyAction, contextClassifier.EmptyDomain }, _keyFactory.Create);
            contextInfoData.Add(null, emptyKey);
        }
    }
}