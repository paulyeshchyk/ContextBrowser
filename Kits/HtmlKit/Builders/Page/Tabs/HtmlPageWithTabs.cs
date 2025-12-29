using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page.CoHtmlElementBuilders;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Builders.Page.Tabs;

//context: build, html
public abstract class HtmlPageWithTabsBuilder<DTO, TTensor>
    where TTensor : notnull
{
    protected readonly IContextInfoDataset<ContextInfo, TTensor> _contextInfoDataset;
    protected readonly HtmlTabbedPageBuilder<DTO> _tabbedPageBuilder;

    protected HtmlPageWithTabsBuilder(IContextInfoDataset<ContextInfo, TTensor> contextInfoDataset, HtmlTabbedPageBuilder<DTO> tabbedPageBuilder)
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

    public async Task GenerateFileAsync(string title, string filename, DTO cellData, CancellationToken cancellationToken)
    {
        var filePath = _exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.pages, filename);

        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        await HtmlBuilderFactory.Raw.CellAsync(writer, innerHtml: "<!DOCTYPE html>", cancellationToken: cancellationToken).ConfigureAwait(false);
        await HtmlBuilderFactory.Html.WithAsync(writer, async (token) =>
        {
            await HtmlBuilderFactory.Head.WithAsync(writer, async (token) =>
            {
                var attributes = new HtmlTagAttributes() { { "charset", "UTF-8" } };
                await HtmlBuilderFactory.Meta.CellAsync(writer, attributes: attributes, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Title.CellAsync(writer, innerHtml: title, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Script.CellAsync(writer, innerHtml: HtmlBuilderFactory.JsScripts.JsShowTabsheetTabScript, isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Script.CellAsync(writer, innerHtml: Resources.JsOpenRequestedTab, isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Style.CellAsync(writer, innerHtml: HtmlBuilderFactory.CssStyles.CssBase, isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Style.CellAsync(writer, innerHtml: HtmlBuilderFactory.CssStyles.CssTabsheet, isEncodable: false, cancellationToken: token).ConfigureAwait(false);
            }, token).ConfigureAwait(false);

            var navItem = new BreadcrumbNavigationItem(filename, title);
            await HtmlBuilderFactory.Body.WithAsync(writer, async (token) =>
            {
                await HtmlBuilderFactory.Breadcrumb(navItem).CellAsync(writer, cancellationToken: token).ConfigureAwait(false);

                await HtmlBuilderFactory.P.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);

                var attributes = new HtmlTagAttributes() { { "class", "tabs-container" } };

                await HtmlBuilderFactory.Div.WithAsync(writer, attributes, (token) =>
                {
                    return HtmlTabsheetBuilder.BuildAsync(writer, _tabsheetDataProvider, cellData, token);
                }, token).ConfigureAwait(false);
            }, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }
}