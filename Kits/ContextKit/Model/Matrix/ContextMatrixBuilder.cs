using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model.Matrix;

// context: ContextInfoMatrix, build
public class ContextMatrixBuilder
{
    private readonly IContextClassifier _contextClassifier;
    private readonly ExportMatrixOptions _matrixOptions;

    public ContextMatrixBuilder(IContextClassifier contextClassifier, ExportMatrixOptions matrixOptions)
    {
        _contextClassifier = contextClassifier;
        _matrixOptions = matrixOptions;
    }

    // context: ContextInfoMatrix, build
    public IContextInfoMatrix BuildMatrix(List<ContextInfo> elements)
    {
        var matrix = new ContextInfoMatrix();

        PopulateMatrixWithElements(matrix, elements);

        AddEmptyCells(matrix, elements);

        return matrix;
    }

    private void PopulateMatrixWithElements(IContextInfoMatrix matrix, List<ContextInfo> elements)
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
                        matrix.Add(item, new ContextInfoMatrixCell(Action: action, Domain: domain));
            }
            else if (verbs.Any())
            {
                foreach (var action in verbs)
                    matrix.Add(item, new ContextInfoMatrixCell(Action: action, Domain: _contextClassifier.EmptyDomain));
            }
            else if (nouns.Any() && includeUnclassified)
            {
                foreach (var domain in nouns)
                    matrix.Add(item, new ContextInfoMatrixCell(Action: _contextClassifier.EmptyAction, Domain: domain));
            }
            else if (includeUnclassified)
            {
                matrix.Add(item, new ContextInfoMatrixCell(Action: _contextClassifier.EmptyAction, Domain: _contextClassifier.EmptyDomain));
            }
        }
    }

    private void AddEmptyCells(IContextInfoMatrix matrix, List<ContextInfo> elements)
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
                matrix.Add(null, new ContextInfoMatrixCell(Action: action, Domain: domain));
            }
        }

        if (includeUnclassified)
        {
            foreach (var noun in allNouns)
            {
                matrix.Add(null, new ContextInfoMatrixCell(Action: _contextClassifier.EmptyAction, Domain: noun));
            }

            matrix.Add(null, new ContextInfoMatrixCell(Action: _contextClassifier.EmptyAction, Domain: _contextClassifier.EmptyDomain));
        }
    }
}