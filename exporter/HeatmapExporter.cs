﻿using System.Text;

namespace ContextBrowser.exporter;

// context: heatmap, build
public static class HeatmapExporter
{
    //context: build, csv, file, heatmap
    public static void GenerateHeatmapCsv(Dictionary<ContextContainer, List<string>> matrix, string outputPath, UnclassifiedPriority unclassifiedPriority = UnclassifiedPriority.None)
    {
        var lines = new List<string>();

        // Отфильтруем обычные ключи
        var filteredKeys = matrix.Keys
            .Where(k => k.Action != ContextClassifier.EmptyAction && k.Domain != ContextClassifier.EmptyDomain)
            .ToList();

        var actions = filteredKeys.Select(k => k.Action).Distinct().OrderBy(x => x).ToList();
        var domains = filteredKeys.Select(k => k.Domain).Distinct().OrderBy(x => x).ToList();

        // Заголовок
        lines.Add("Action;" + string.Join(";", domains));

        // Основная матрица
        foreach(var action in actions)
        {
            var row = new List<string> { action };
            foreach(var domain in domains)
            {
                var key = (action, domain);
                var count = matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
                row.Add(count.ToString());
            }
            lines.Add(string.Join(";", row));
        }

        var includeUnclassified = (unclassifiedPriority != UnclassifiedPriority.None);
        // Добавим строку для нераспознанных, если нужно
        if(includeUnclassified && matrix.ContainsKey((ContextClassifier.EmptyAction, ContextClassifier.EmptyDomain)))
        {
            var unclassifiedCount = matrix[(ContextClassifier.EmptyAction, ContextClassifier.EmptyDomain)].Count;
            var row = new List<string> { ContextClassifier.EmptyAction };

            // Заполняем пустыми значениями для всех доменов
            foreach(var _ in domains)
                row.Add("0");

            // Добавим колонку "NoDomain" в конец, если она не была в списке
            row.Add(unclassifiedCount.ToString());

            // Обновим заголовок, добавив "NoDomain" в конец
            if(!domains.Contains(ContextClassifier.EmptyDomain))
                lines[0] += $";{ContextClassifier.EmptyDomain}";

            lines.Add(string.Join(";", row));
        }

        File.WriteAllLines(outputPath, lines);
    }


    //context: build, html, page, directory, uml
    public static void GenerateContextHtmlPages(Dictionary<ContextContainer, List<string>> matrix, string outputDirectory)
    {
        foreach(var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var compositeFileName = $"composite_{action}_{domain}.html";
            var filePath = Path.Combine(outputDirectory, compositeFileName);

            string documentTitle = $" {action}  →  {domain} ";
            var pumlFileName = $"composite_{action}_{domain}.puml";
            var pumlFilePath = Path.Combine(outputDirectory, pumlFileName);
            var pumlFileContent = ReadPumlContent(pumlFilePath);
            var pumlEmbeddedScript = "<script type=\"module\">import enableElement from \"https://cdn.pika.dev/render-plantuml\";enableElement();</script>";
            var pumlEmbeddedRenderer = $"<render-plantuml renderMode=\"txt\">{pumlFileContent}</render-plantuml>";

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine($"<head><meta charset=\"UTF-8\"><title>{documentTitle}</title>");
            sb.AppendLine(pumlEmbeddedScript);
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h1>{action.ToUpper()} → {domain}</h1>");
            sb.AppendLine($"<p>Methods: {cell.Value.Count}</p>");
            sb.AppendLine("<ul>");

            foreach(var method in cell.Value.Distinct())
            {
                sb.AppendLine($"  <li>{method}</li>");
            }

            sb.AppendLine("</ul>");

            sb.AppendLine(pumlEmbeddedRenderer);

            sb.AppendLine("</body></html>");

            File.WriteAllText(filePath, sb.ToString());
        }
    }


    public static string ReadPumlContent(string pumlFilePath)
    {
        if(!File.Exists(pumlFilePath))
            throw new FileNotFoundException("Файл не найден", pumlFilePath);

        // Чтение всего файла в строку
        return File.ReadAllText(pumlFilePath);
    }

    //context: build, html, page, directory, uml
    public static void GenerateContextDimensionHtmlPages(Dictionary<ContextContainer, List<string>> matrix, string outputDirectory)
    {
        var allActions = matrix.Keys.Select(k => k.Action).Distinct();
        var allDomains = matrix.Keys.Select(k => k.Domain).Distinct();

        foreach(var action in allActions)
        {
            var methods = matrix
                .Where(kvp => kvp.Key.Action == action)
                .SelectMany(kvp => kvp.Value)
                .Distinct();
            var actionFileName = $"action_{action}.html";
            var path = Path.Combine(outputDirectory, actionFileName);

            var sb = new StringBuilder();
            sb.AppendLine($"<html><head><meta charset=\"UTF-8\"><title>Action: {action}</title></head><body>");
            sb.AppendLine($"<h1>Action: {action}</h1>");
            sb.AppendLine($"<p>Methods: {methods.Count()}</p><ul>");

            foreach(var method in methods)
                sb.AppendLine($"<li>{method}</li>");

            sb.AppendLine("</ul></body></html>");
            File.WriteAllText(path, sb.ToString());
        }

        foreach(var domain in allDomains)
        {
            var methods = matrix
                .Where(kvp => kvp.Key.Domain == domain)
                .SelectMany(kvp => kvp.Value)
                .Distinct();

            var domainFileName = $"domain_{domain}.html";
            var path = Path.Combine(outputDirectory, domainFileName);

            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset=\"UTF-8\"><title>Domain: " + domain + "</title></head><body>");
            sb.AppendLine($"<h1>Domain: {domain}</h1>");
            sb.AppendLine($"<p>Methods: {methods.Count()}</p><ul>");

            foreach(var method in methods)
                sb.AppendLine($"<li>{method}</li>");

            sb.AppendLine("</ul></body></html>");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
