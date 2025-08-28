using ContextBrowserKit.Options.Export;
using ContextKit.Model.Matrix;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using System.Text;
using HtmlKit.Model;
using HtmlKit.Builders.Page;
using HtmlKit.Model.Tabsheet;
using ContextKit.Model;

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
            var filePath = ExportPathBuilder.BuildPath(_exportOptions.Paths, ExportPathType.pages, filename);
            var title = $" {cellData.DataCell.Action}  ->  {cellData.DataCell.Domain} ";

            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            HtmlBuilderFactory.Raw.Cell(writer, "<!DOCTYPE html>");
            HtmlBuilderFactory.Html.With(writer, () =>
            {
                HtmlBuilderFactory.Head.With(writer, () =>
                {
                    HtmlBuilderFactory.Meta.Cell(writer, style: "charset=\"UTF-8\"");


                    HtmlBuilderFactory.Title.Cell(writer, title);

                    HtmlBuilderFactory.Script.Cell(writer, HtmlBuilderFactory.JsScripts.JsShowTabsheetTabScript);
                    HtmlBuilderFactory.Style.Cell(writer, HtmlBuilderFactory.CssStyles.CssTabsheet);
                });

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.P.Cell(writer, "index", "..\\index.html");

                    HtmlBuilderFactory.Div.With(writer, () =>
                    {
                        HtmlTabsheetBuilder.Build(writer, _tabsheetDataProvider, cellData);
                    }, className: "tabs-container");
                });
            });
        }
    }
}
