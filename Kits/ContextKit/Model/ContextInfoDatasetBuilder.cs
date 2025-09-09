using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier);
}

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilder : IContextInfoDatasetBuilder
{
    private readonly IAppLogger<AppLevel> _logger;

    public ContextInfoDatasetBuilder(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var elements = contextsList.ToList();
        var map = new ContextInfoDataset();

        PopulateMatrixWithElements(map, elements, matrixOptions, contextClassifier);

        AddEmptyCells(map, elements, matrixOptions, contextClassifier);

        return map;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    internal static void PopulateMatrixWithElements(IContextInfoDataset contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions, IContextClassifier contextClassifier)
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

    private static void AddEmptyCells(IContextInfoDataset contextInfoData, List<ContextInfo> elements, ExportMatrixOptions _matrixOptions, IContextClassifier contextClassifier)
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