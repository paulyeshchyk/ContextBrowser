using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextKit.Model;
using TensorKit.Model;

namespace ExporterKit.Csv;

public static class ContextInfoCsvExporter
{
    //context: build, csv, matrix
    public static void GenerateCsv(IDomainPerActionContextTensorClassifier contextClassifier, Dictionary<DomainPerActionTensor, List<string>> matrix, string outputPath)
    {
        var lines = new List<string>();
        lines.AddRange(contextClassifier.MetaItems);

        foreach (var cell in matrix)
        {
            BuildItem(lines, cell);
        }

        File.WriteAllLines(outputPath, lines);
    }

    private static void BuildItem(List<string> lines, KeyValuePair<DomainPerActionTensor, List<string>> cell)
    {
        var cellInfo = cell.Key;
        var items = cell.Value.Any()
            ? string.Join(", ", cell.Value.Distinct())
            : string.Empty;

        lines.Add($"{cellInfo.Action};{cellInfo.Domain};{items}");
    }
}