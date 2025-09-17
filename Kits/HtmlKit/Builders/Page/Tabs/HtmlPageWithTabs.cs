using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;
using Microsoft.VisualBasic;

namespace ExporterKit.Html;

//context: build, html
public abstract class HtmlPageWithTabsBuilder<DTO>
{
    protected readonly IContextInfoDataset<ContextInfo> _contextInfoDataset;
    protected readonly HtmlTabbedPageBuilder<DTO> _tabbedPageBuilder;

    protected HtmlPageWithTabsBuilder(IContextInfoDataset<ContextInfo> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder)
    {
        _contextInfoDataset = contextInfoDataset;
        _tabbedPageBuilder = tabbedPageBuilder;
    }

    public abstract Task BuildAsync(CancellationToken cancellationToken);
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
                HtmlBuilderFactory.Script.Cell(writer, innerHtml: Resources.JsOpenRequestedTab, isEncodable: false);
                HtmlBuilderFactory.Style.Cell(writer, innerHtml: HtmlBuilderFactory.CssStyles.CssBase, isEncodable: false);
                HtmlBuilderFactory.Style.Cell(writer, innerHtml: HtmlBuilderFactory.CssStyles.CssTabsheet, isEncodable: false);
            });

            var navItem = new BreadcrumbNavigationItem(filename, title);
            HtmlBuilderFactory.Body.With(writer, () =>
            {
                HtmlBuilderFactory.Breadcrumb(navItem).Cell(writer);

                HtmlBuilderFactory.P.Cell(writer);

                var attributes = new HtmlTagAttributes() { { "class", "tabs-container" } };

                HtmlBuilderFactory.Div.With(writer, attributes, () =>
                {
                    HtmlTabsheetBuilder.Build(writer, _tabsheetDataProvider, cellData);
                });
            });
        });
    }
}