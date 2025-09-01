using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model.Matrix;

// context: ContextInfoMatrix, build
public class ContextInfoDataBuilder
{
    private readonly IContextClassifier _contextClassifier;
    private readonly ExportMatrixOptions _matrixOptions;

    public ContextInfoDataBuilder(IContextClassifier contextClassifier, ExportMatrixOptions matrixOptions)
    {
        _contextClassifier = contextClassifier;
        _matrixOptions = matrixOptions;
    }

    // context: ContextInfoMatrix, build
    public IContextInfoData BuildMatrix(List<ContextInfo> elements)
    {
        var contextInfoData = new ContextInfoData();

        PopulateMatrixWithElements(contextInfoData, elements);

        AddEmptyCells(contextInfoData, elements);

        return contextInfoData;
    }

    private void PopulateMatrixWithElements(IContextInfoData contextInfoData, List<ContextInfo> elements)
    {
        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        foreach (var item in elements)
        {
            var verbs = item.Contexts.Where(_contextClassifier.IsVerb).Distinct().ToList();
            var nouns = item.Contexts.Where(_contextClassifier.IsNoun).Distinct().ToList();

            if (verbs.Any() && nouns.Any())
            {
                foreach (var action in verbs)
                    foreach (var domain in nouns)
                        contextInfoData.Add(item, new ContextInfoDataCell(Action: action, Domain: domain));
            }
            else if (verbs.Any())
            {
                foreach (var action in verbs)
                    contextInfoData.Add(item, new ContextInfoDataCell(Action: action, Domain: _contextClassifier.EmptyDomain));
            }
            else if (nouns.Any() && includeUnclassified)
            {
                foreach (var domain in nouns)
                    contextInfoData.Add(item, new ContextInfoDataCell(Action: _contextClassifier.EmptyAction, Domain: domain));
            }
            else if (includeUnclassified)
            {
                contextInfoData.Add(item, new ContextInfoDataCell(Action: _contextClassifier.EmptyAction, Domain: _contextClassifier.EmptyDomain));
            }
        }
    }

    private void AddEmptyCells(IContextInfoData contextInfoData, List<ContextInfo> elements)
    {
        if (!_matrixOptions.IncludeAllStandardActions) return;

        var allVerbs = _contextClassifier.GetCombinedVerbs(elements.SelectMany(e => e.Contexts).Where(_contextClassifier.IsVerb).Distinct().ToList()).ToList();
        var allNouns = elements.SelectMany(e => e.Contexts).Where(_contextClassifier.IsNoun).Distinct().ToList();

        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        var nounsToUse = includeUnclassified
            ? allNouns.Append(_contextClassifier.EmptyDomain)
            : allNouns;

        foreach (var action in allVerbs)
        {
            foreach (var domain in nounsToUse)
            {
                contextInfoData.Add(null, new ContextInfoDataCell(Action: action, Domain: domain));
            }
        }

        if (includeUnclassified)
        {
            foreach (var noun in allNouns)
            {
                contextInfoData.Add(null, new ContextInfoDataCell(Action: _contextClassifier.EmptyAction, Domain: noun));
            }

            contextInfoData.Add(null, new ContextInfoDataCell(Action: _contextClassifier.EmptyAction, Domain: _contextClassifier.EmptyDomain));
        }
    }
}