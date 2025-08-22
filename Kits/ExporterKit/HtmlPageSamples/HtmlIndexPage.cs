using ContextBrowserKit.Options.Export;
using ContextKit.Model.Matrix;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using System.Text;

namespace ExporterKit.HtmlPageSamples;

//context: build, html
public static class HtmlIndexPage
{
    //context: build, html, page, directory
    public static void GenerateContextHtmlPages(IContextInfoMatrix matrix, ExportOptions exportOptions)
    {
        foreach (var cell in matrix)
        {
            var (action, domain) = cell.Key;
            var filePath = ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.pages, $"composite_{action}_{domain}.html");

            var title = $" {action}  ->  {domain} ";
            var methodCount = cell.Value.Count;
            var distinctMethods = cell.Value.Distinct();

            var injection = PumlInjector.InjectActionPerDomainEmbeddedHtml(action, domain, exportOptions);

            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            HtmlBuilderFactory.Raw.Cell(writer, "<!DOCTYPE html>");
            HtmlBuilderFactory.Html.With(writer, () =>
            {
                HtmlBuilderFactory.Head.With(writer, () =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                    HtmlBuilderFactory.Title.Cell(writer, title);

                    if (!string.IsNullOrWhiteSpace(injection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, injection.EmbeddedScript);
                });

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"{action.ToUpper()} -> {domain}");
                    HtmlBuilderFactory.Paragraph.Cell(writer, $"Methods: {methodCount}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in distinctMethods)
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });

                    if (!string.IsNullOrWhiteSpace(injection.EmbeddedContent))
                        HtmlBuilderFactory.Raw.Cell(writer, injection.EmbeddedContent);
                });
            });
        }
    }
}