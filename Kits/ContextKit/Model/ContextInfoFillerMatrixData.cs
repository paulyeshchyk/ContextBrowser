using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoFillerMatrixData : IContextInfoFiller
{
    public int Order { get; } = int.MinValue;

    // context: ContextInfo, ContextInfoMatrix, build
    public void Fill(IContextInfoDataset<ContextInfo> contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions, IContextClassifier contextClassifier)
    {
        bool includeUnclassified = _matrixOptions.UnclassifiedPriority != UnclassifiedPriorityType.None;

        foreach (var item in elements)
        {
            var verbs = item.Contexts.Where(contextClassifier.IsVerb).Distinct().ToList();
            var nouns = item.Contexts.Where(contextClassifier.IsNoun).Distinct().ToList();

            if (verbs.Any() && nouns.Any())
            {
                foreach (var action in verbs)
                    foreach (var domain in nouns)
                        contextInfoData.Add(item, new ContextKey(Action: action, Domain: domain));
            }
            else if (verbs.Any())
            {
                foreach (var action in verbs)
                    contextInfoData.Add(item, new ContextKey(Action: action, Domain: contextClassifier.EmptyDomain));
            }
            else if (nouns.Any() && includeUnclassified)
            {
                foreach (var domain in nouns)
                    contextInfoData.Add(item, new ContextKey(Action: contextClassifier.EmptyAction, Domain: domain));
            }
            else if (includeUnclassified)
            {
                contextInfoData.Add(item, new ContextKey(Action: contextClassifier.EmptyAction, Domain: contextClassifier.EmptyDomain));
            }
        }
    }
}
