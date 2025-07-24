using ContextBrowser.ContextKit.Model;
using ContextBrowser.exporter.Puml;
using System.Text;

namespace ContextBrowser.exporter.HtmlPageSamples;

public static class HtmlDimensionPage
{
    //context: build, html, page, directory, uml
    public static void GenerateContextDimensionHtmlPages(Dictionary<ContextContainer, List<string>> matrix, string outputDirectory, Func<string, bool> domainCallback)
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

            var embeddedScript = string.Empty;
            var embeddedContent = string.Empty;
            if(domainCallback(domain))
            {
                var pumlInjection = PumlInjector.InjectDomainEmbeddedHtml(domain, outputDirectory);
                embeddedScript = pumlInjection.EmbeddedScript;
                embeddedContent = pumlInjection.EmbeddedContent;
            }
            else
            {
            }
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset=\"UTF-8\">");
            sb.AppendLine($"<title>Domain: " + domain + "</title>");
            sb.AppendLine(embeddedScript);
            sb.AppendLine("</head><body>");
            sb.AppendLine($"<h1>Domain: {domain}</h1>");
            sb.AppendLine($"<p>Methods: {methods.Count()}</p><ul>");

            foreach(var method in methods)
                sb.AppendLine($"<li>{method}</li>");

            sb.AppendLine("</ul>");

            sb.AppendLine(embeddedContent);
            sb.AppendLine("</body></html>");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
