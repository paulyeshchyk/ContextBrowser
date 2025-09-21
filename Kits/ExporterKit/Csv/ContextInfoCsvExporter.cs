using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Csv;

public static class ContextInfoCsvExporter<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    //context: build, csv, matrix
    public static void GenerateCsv(ITensorClassifierDomainPerActionContext contextClassifier, Dictionary<TDataTensor, List<string>> matrix, string outputPath)
    {
        var lines = new List<string>();
        lines.AddRange(contextClassifier.MetaItems);

        foreach (var cell in matrix)
        {
            BuildItem(lines, cell);
        }

        File.WriteAllLines(outputPath, lines);
    }

    private static void BuildItem(List<string> lines, KeyValuePair<TDataTensor, List<string>> cell)
    {
        var cellInfo = cell.Key;
        var items = cell.Value.Any()
            ? string.Join(", ", cell.Value.Distinct())
            : string.Empty;

        lines.Add($"{cellInfo.Action};{cellInfo.Domain};{items}");
    }
}