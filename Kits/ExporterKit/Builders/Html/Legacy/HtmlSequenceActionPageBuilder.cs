using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using System.Text;

namespace ExporterKit.HtmlPageSamples;

//context: build, html
public static class HtmlSequenceActionPageBuilder
{
    //context: build, html
    public static void Build(string action, List<ContextInfo> methods, PumlHtmlInjection? puml, ExportOptions _exportOptions)
    {
        var path = ExportPathBuilder.BuildPath(_exportOptions.Paths, ExportPathType.pages, $"composite_action_{action}.html");

        using var writer = new StreamWriter(path, false, Encoding.UTF8);

        HtmlBuilderFactory.Html.With(writer, () =>
        {
            HtmlBuilderFactory.Head.With(writer, () =>
            {
                HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");
                HtmlBuilderFactory.Title.Cell(writer, $"Action: {action}");

                HtmlBuilderFactory.Script.Cell(writer, HtmlBuilderFactory.JsScripts.JsShowTabsheetTabScript);
                HtmlBuilderFactory.Style.Cell(writer, HtmlBuilderFactory.CssStyles.CssTabsheet);

                var script = puml?.EmbeddedScript;
                if (!string.IsNullOrWhiteSpace(script))
                    HtmlBuilderFactory.Raw.Cell(writer, script);
            });

            HtmlBuilderFactory.Body.With(writer, () =>
            {
                HtmlBuilderFactory.P.Cell(writer, "index", "..\\index.html");

                HtmlBuilderFactory.Div.With(writer, () =>
                {
                    HtmlBuilderFactory.Div.With(writer, () =>
                    {
                        HtmlBuilderFactory.Button.OnClick("showTab('content1', this)").Cell(writer, "Методы", className: "tab-button active");
                        HtmlBuilderFactory.Button.OnClick("showTab('content2', this)").Cell(writer, "Последовательность", className: "tab-button");
                    }, className: "tabs");
                }, className: "tabs-container");

                HtmlBuilderFactory.Div.With(writer, () =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, $"Action: {action}");
                    HtmlBuilderFactory.P.Cell(writer, $"Methods: {methods.Count}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods)
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });
                }, className: "tab-content active", id: "content1");

                HtmlBuilderFactory.Div.With(writer, () =>
                {
                    var embeddedContent = puml?.EmbeddedContent;
                    if (!string.IsNullOrWhiteSpace(embeddedContent))
                    {
                        HtmlBuilderFactory.Raw.Cell(writer, embeddedContent);
                    }
                    else
                    {
                        HtmlBuilderFactory.Raw.Cell(writer, "no content");
                    }
                }, className: "tab-content", id: "content2");
            });
        });
    }
}