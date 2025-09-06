using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace ExporterKit.Html;

//context: build, html
public abstract class HtmlPageWithTabsBuilder<DTO>
{
    protected readonly IContextInfoDataset _contextInfoDataset;
    protected readonly HtmlTabbedPageBuilder<DTO> _tabbedPageBuilder;

    protected HtmlPageWithTabsBuilder(IContextInfoDataset contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder)
    {
        _contextInfoDataset = contextInfoDataset;
        _tabbedPageBuilder = tabbedPageBuilder;
    }

    public abstract void Build();
}

public class HtmlTabbedPageBuilder<DTO>
{
    private readonly ExportOptions _exportOptions;
    private readonly IHtmlTabsheetDataProvider<DTO> _tabsheetDataProvider;

    public HtmlTabbedPageBuilder(ExportOptions exportOptions, IHtmlTabsheetDataProvider<DTO> tabsheetDataProvider)
    {
        _exportOptions = exportOptions;
        _tabsheetDataProvider = tabsheetDataProvider;
    }

    public void GenerateFile(string title, string filename, DTO cellData)
    {
        var filePath = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pages, filename);

        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        HtmlBuilderFactory.Raw.Cell(writer, innerHtml: "<!DOCTYPE html>");
        HtmlBuilderFactory.Html.With(writer, () =>
        {
            HtmlBuilderFactory.Head.With(writer, () =>
            {
                var attributes = new HtmlTagAttributes() { { "charset", "UTF-8" } };
                HtmlBuilderFactory.Meta.Cell(writer, attributes: attributes);
                HtmlBuilderFactory.Title.Cell(writer, innerHtml: title);
                HtmlBuilderFactory.Script.Cell(writer, innerHtml: HtmlBuilderFactory.JsScripts.JsShowTabsheetTabScript, isEncodable: false);
                HtmlBuilderFactory.Style.Cell(writer, innerHtml: HtmlBuilderFactory.CssStyles.CssTabsheet, isEncodable: false);
            });

            HtmlBuilderFactory.Body.With(writer, () =>
            {
                HtmlBuilderFactory.P.With(writer, () =>
                {
                    var attributes = new HtmlTagAttributes() { { "href", "..\\index.html" } };
                    HtmlBuilderFactory.A.Cell(writer, attributes, "index");
                });

                var attributes = new HtmlTagAttributes() { { "class", "tabs-container" } };

                HtmlBuilderFactory.Div.With(writer, attributes, () =>
                {
                    HtmlTabsheetBuilder.Build(writer, _tabsheetDataProvider, cellData);
                });
            });
        });
    }
}