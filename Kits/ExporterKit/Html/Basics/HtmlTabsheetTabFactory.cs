using ContextBrowserKit.Options.Export;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace ContextBrowser.Samples.HtmlPages;

public static class HtmlTabsheetTabBase
{
    public static HtmlTabsheetTabInfo<ContextKeyContainer> EmptyTab(ExportOptions exportOptions)
    {
        return new HtmlTabsheetTabInfo<ContextKeyContainer>
            (
                info: new TabsheetTabInfo(tabId: "EmptyTab", caption: "Empty"),
                buildHtmlTab: (writer, tabsheetProvider, dto) =>
                {
                    HtmlBuilderFactory.P.Cell(writer, innerHtml: "Empty");
                },
                isActive: false
            )
        { };
    }
}
