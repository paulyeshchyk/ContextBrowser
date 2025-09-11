using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFillerEmptydata : IContextInfoFiller
{
    public int Order { get; } = int.MaxValue;

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo> contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions, IContextClassifier contextClassifier)
    {
        if (!_matrixOptions.IncludeAllStandardActions)
            return;

        var allVerbs = contextClassifier.GetCombinedVerbs(elements.SelectMany(e => e.Contexts).Where(contextClassifier.IsVerb).Distinct().ToList()).ToList();
        var allNouns = elements.SelectMany(e => e.Contexts).Where(contextClassifier.IsNoun).Distinct().ToList();

        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        var nounsToUse = includeUnclassified
            ? allNouns.Append(contextClassifier.EmptyDomain)
            : allNouns;

        foreach (var action in allVerbs)
        {
            foreach (var domain in nounsToUse)
            {
                contextInfoData.Add(null, new ContextKey(Action: action, Domain: domain));
            }
        }

        if (includeUnclassified)
        {
            foreach (var noun in allNouns)
            {
                contextInfoData.Add(null, new ContextKey(Action: contextClassifier.EmptyAction, Domain: noun));
            }

            contextInfoData.Add(null, new ContextKey(Action: contextClassifier.EmptyAction, Domain: contextClassifier.EmptyDomain));
        }
    }
}