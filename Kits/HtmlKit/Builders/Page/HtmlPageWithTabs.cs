using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace ExporterKit.Html;

//context: build, html
public class HtmlPageWithTabsBuilder
{
    private readonly IContextInfoDataset _contextInfoDataset;
    private readonly ExportOptions _exportOptions;
    private readonly IHtmlTabsheetDataProvider _tabsheetDataProvider;

    public HtmlPageWithTabsBuilder(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions, IHtmlTabsheetDataProvider tabsheetDataProvider)
    {
        _contextInfoDataset = contextInfoDataset;
        _exportOptions = exportOptions;
        _tabsheetDataProvider = tabsheetDataProvider;
    }

    //context: build, html
    public void Build(Func<HtmlContextInfoDataCell, string> onGetFileName)
    {
        var contextInfoData = _contextInfoDataset.ContextInfoData;

        foreach (var contextInfoItem in contextInfoData)
        {
            var (action, domain) = contextInfoItem.Key;

            var cellData = new HtmlContextInfoDataCell
            (
                dataCell: contextInfoItem.Key,
                 methods: contextInfoItem.Value.Distinct()
            );

            var filename = onGetFileName(cellData);
            var filePath = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pages, filename);
            var title = $" {cellData.DataCell.Action}  ->  {cellData.DataCell.Domain} ";

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
}
