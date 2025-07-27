using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.exporter;

// context: uml, build, csv
// pattern: Builder
// pattern note: strategic tool
public static class ContextMatrixUmlExporter
{
    //context: build, uml, matrix
    public static Dictionary<ContextContainer, List<string>> GenerateMatrix(List<ContextInfo> elements, IContextClassifier contextClassifier, UnclassifiedPriority unclassifiedPriority = UnclassifiedPriority.None, bool includeAllStandardActions = false)
    {
        var matrix = new Dictionary<ContextContainer, List<string>>();

        var allVerbs = elements
            .SelectMany(e => e.Contexts)
            .Where(contextClassifier.IsVerb)
            .Distinct()
            .ToList();

        var allNouns = elements
            .SelectMany(e => e.Contexts)
            .Where(contextClassifier.IsNoun)
            .Distinct()
            .ToList();

        // Расширим список действий, если нужно
        if(includeAllStandardActions && contextClassifier is ContextClassifier classifier)
        {
            foreach(var verb in classifier.StandardActions)
            {
                if(!allVerbs.Contains(verb))
                    allVerbs.Add(verb);
            }
        }

        bool includeUnclassified = (unclassifiedPriority != UnclassifiedPriority.None);

        foreach(var item in elements)
        {
            var verbs = item.Contexts.Where(contextClassifier.IsVerb).Distinct().ToList();
            var nouns = item.Contexts.Where(contextClassifier.IsNoun).Distinct().ToList();

            var label = item.ElementType == ContextInfoElementType.method && item.ClassOwner != null
                ? $"{item.ClassOwner}.{item.Name}"
                : item.Name;

            if(string.IsNullOrWhiteSpace(label))
                continue;

            if(verbs.Any() && nouns.Any())
            {
                foreach(var verb in verbs)
                    foreach(var noun in nouns)
                    {
                        var key = (verb, noun);
                        if(!matrix.ContainsKey(key))
                            matrix[key] = new List<string>();

                        matrix[key].Add(label);
                    }
            }
            else if(verbs.Any()) // Только действия
            {
                foreach(var verb in verbs)
                {
                    var key = (verb, ContextClassifier.EmptyDomain);
                    if(!matrix.ContainsKey(key))
                        matrix[key] = new List<string>();

                    matrix[key].Add(label);
                }
            }
            else if(nouns.Any() && includeUnclassified) // Только домены, но разрешено
            {
                foreach(var noun in nouns)
                {
                    var key = (ContextClassifier.EmptyAction, noun);
                    if(!matrix.ContainsKey(key))
                        matrix[key] = new List<string>();

                    matrix[key].Add(label);
                }
            }
            else if(includeUnclassified) // Полностью без контекста
            {
                var key = (ContextClassifier.EmptyAction, ContextClassifier.EmptyDomain);
                if(!matrix.ContainsKey(key))
                    matrix[key] = new List<string>();

                matrix[key].Add(label);
            }
        }

        // Добавим пустые ячейки для всех стандартных действий
        if(includeAllStandardActions)
        {
            foreach(var verb in allVerbs)
            {
                // Добавляем только если разрешено
                var nounsToUse = includeUnclassified
                    ? allNouns.Append(ContextClassifier.EmptyDomain)
                    : allNouns;

                foreach(var noun in nounsToUse)
                {
                    var key = (verb, noun);
                    if(!matrix.ContainsKey(key))
                        matrix[key] = new List<string>();
                }
            }

            if(includeUnclassified)
            {
                foreach(var noun in allNouns)
                {
                    var key = (ContextClassifier.EmptyAction, noun);
                    if(!matrix.ContainsKey(key))
                        matrix[key] = new List<string>();
                }

                var keyEmpty = (ContextClassifier.EmptyAction, ContextClassifier.EmptyDomain);
                if(!matrix.ContainsKey(keyEmpty))
                    matrix[keyEmpty] = new List<string>();
            }
        }
        return matrix;
    }

    //context: build, uml, links
    public static HashSet<(string From, string To)> GenerateMethodLinks(List<ContextInfo> elements, IContextClassifier contextClassifier)
    {
        var methodToCell = new Dictionary<string, string>();

        foreach(var item in elements.Where(e => e.ElementType == ContextInfoElementType.method))
        {
            string? a = item.Contexts.FirstOrDefault(c => contextClassifier.IsVerb(c));
            string? d = item.Contexts.FirstOrDefault(c => contextClassifier.IsNoun(c));

            if(a != null && d != null)
            {
                var methodId = item.ClassOwner != null ? $"{item.ClassOwner}.{item.Name}" : item.Name;
                if(methodId != null)
                {
                    var cellId = $"{a}_{d}";
                    methodToCell[methodId] = cellId;
                }
            }
        }

        var theMethods = elements.Where(e => e.ElementType == ContextInfoElementType.method);
        var links = new HashSet<(string From, string To)>();

        foreach(var callerMethod in theMethods)
        {
            // Собираем идентификатор вызывающего метода
            var callerId = callerMethod.ClassOwner != null
                ? $"{callerMethod.ClassOwner}.{callerMethod.Name}"
                : callerMethod.Name;

            if(string.IsNullOrWhiteSpace(callerId))
                continue;

            // Пропускаем, если метод не сопоставлен с контекстной ячейкой
            if(!methodToCell.TryGetValue(callerId, out var fromCell))
                continue;

            foreach(var calledName in callerMethod.References)
            {
                // Ищем вызываемый метод среди всех методов
                var calleeMethod = theMethods.FirstOrDefault(m =>
                    string.Equals(m.Name, calledName.Name, StringComparison.OrdinalIgnoreCase));

                if(calleeMethod == null)
                    continue;

                var calleeId = calleeMethod.ClassOwner != null
                    ? $"{calleeMethod.ClassOwner}.{calleeMethod.Name}"
                    : calleeMethod.Name;
                if(string.IsNullOrWhiteSpace(calleeId))
                    continue;
                // Пропускаем, если вызываемый метод не сопоставлен
                if(!methodToCell.TryGetValue(calleeId, out var toCell))
                    continue;

                // Добавляем направленную связь, если ячейки разные
                if(fromCell != toCell)
                    links.Add((fromCell, toCell));
            }
        }
        return links;
    }


    //context: build, csv, matrix
    public static void GenerateCsv(Dictionary<ContextContainer, List<string>> matrix, string outputPath)
    {
        var lines = new List<string>();
        lines.AddRange(ContextClassifier.MetaItems);

        foreach(var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var items = cell.Value.Any()
                ? string.Join(", ", cell.Value.Distinct())
                : string.Empty;

            lines.Add($"{action};{domain};{items}");
        }

        File.WriteAllLines(outputPath, lines);
    }
}