using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace ExporterKit.Csv;

// context: heatmap, build
// pattern: Builder
public static class CsvGenerator
{
    //context: build, csv, heatmap
    public static void GenerateHeatmapCsv(IContextClassifier contextClassifier, Dictionary<ContextInfoDataCell, List<string>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None)
    {
        var lines = new List<string>();

        // Отфильтруем обычные ключи
        var filteredKeys = matrix.Keys
            .Where(k => k.Action != contextClassifier.EmptyAction && k.Domain != contextClassifier.EmptyDomain)
            .ToList();

        var actions = filteredKeys.Select(k => k.Action).Distinct().OrderBy(x => x).ToList();
        var domains = filteredKeys.Select(k => k.Domain).Distinct().OrderBy(x => x).ToList();

        // Заголовок
        lines.Add("Action;" + string.Join(";", domains));

        // Основная матрица
        foreach (var action in actions)
        {
            var row = new List<string> { action };
            foreach (var domain in domains)
            {
                var key = (action, domain);
                var count = matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
                row.Add(count.ToString());
            }
            lines.Add(string.Join(";", row));
        }

        var includeUnclassified = unclassifiedPriority != UnclassifiedPriorityType.None;

        // Добавим строку для нераспознанных, если нужно
        if (includeUnclassified && matrix.ContainsKey((contextClassifier.EmptyAction, contextClassifier.EmptyDomain)))
        {
            var unclassifiedCount = matrix[(contextClassifier.EmptyAction, contextClassifier.EmptyDomain)].Count;
            var row = new List<string> { contextClassifier.EmptyAction };

            // Заполняем пустыми значениями для всех доменов
            foreach (var _ in domains)
                row.Add("0");

            // Добавим колонку "NoDomain" в конец, если она не была в списке
            row.Add(unclassifiedCount.ToString());

            // Обновим заголовок, добавив "NoDomain" в конец
            if (!domains.Contains(contextClassifier.EmptyDomain))
                lines[0] += $";{contextClassifier.EmptyDomain}";

            lines.Add(string.Join(";", row));
        }

        File.WriteAllLines(outputPath, lines);
    }
}