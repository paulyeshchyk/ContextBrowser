using ContextBrowser.ContextKit.Model;
using ContextBrowser.exporter.Puml;
using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Page;
using System.Text;

namespace ContextBrowser.exporter.HtmlPageSamples;

public static class HtmlDimensionPage
{
    public static void GenerateContextDimensionHtmlPages(Dictionary<ContextContainer, List<string>> matrix, string outputDirectory, Func<string, bool> domainCallback)
    {
        var allActions = matrix.Keys.Select(k => k.Action).Distinct();
        var allDomains = matrix.Keys.Select(k => k.Domain).Distinct();

        // ---------- ACTION ----------
        foreach(var action in allActions)
        {
            var methods = matrix.Where(kvp => kvp.Key.Action == action)
                                .SelectMany(kvp => kvp.Value)
                                .Distinct()
                                .ToList();

            var path = Path.Combine(outputDirectory, $"action_{action}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Action: {action}");
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Action: {action}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });
                });
            });
        }

        // ---------- DOMAIN ----------
        foreach(var domain in allDomains)
        {
            var methods = matrix.Where(kvp => kvp.Key.Domain == domain)
                                .SelectMany(kvp => kvp.Value)
                                .Distinct()
                                .ToList();

            string embeddedScript = string.Empty;
            string embeddedContent = string.Empty;

            if(domainCallback(domain))
            {
                var puml = PumlInjector.InjectDomainEmbeddedHtml(domain, outputDirectory);
                embeddedScript = puml.EmbeddedScript;
                embeddedContent = puml.EmbeddedContent;
            }

            var path = Path.Combine(outputDirectory, $"domain_{domain}.html");

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            HtmlBuilderFactory.Html.With(writer,() =>
            {
                HtmlBuilderFactory.Head.With(writer,() =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, $"Domain: {domain}");

                    if(!string.IsNullOrWhiteSpace(embeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedScript);
                });

                HtmlBuilderFactory.Body.With(writer,() =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Domain: {domain}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer,() =>
                    {
                        foreach(var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method);
                    });

                    if(!string.IsNullOrWhiteSpace(embeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                });
            });
        }
    }
}
