using ContextBrowser.exporter.Puml;
using System.Text;

namespace ContextBrowser.exporter.HtmlPageSamples;

public static class HtmlIndexPage
{
    //context: build, html, page, directory
    public static void GenerateContextHtmlPages(Dictionary<ContextContainer, List<string>> matrix, string outputDirectory)
    {
        foreach(var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var compositeFileName = $"composite_{action}_{domain}.html";
            var filePath = Path.Combine(outputDirectory, compositeFileName);

            string documentTitle = $" {action}  →  {domain} ";
            var pumlInjection = PumlInjector.InjectActionPerDomainEmbeddedHtml(action, domain, outputDirectory);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine($"<head><meta charset=\"UTF-8\"><title>{documentTitle}</title>");
            sb.AppendLine(pumlInjection.EmbeddedScript);
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

            sb.AppendLine(pumlInjection.EmbeddedContent);

            sb.AppendLine("</body></html>");

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
