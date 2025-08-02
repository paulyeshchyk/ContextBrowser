using ContextKit.Model;

namespace ContextBrowser.ExporterKit.Csv;

public static class ContextInfoCsvExporter
{
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