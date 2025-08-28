using HtmlKit.Page;
using HtmlKit.Model;
using ContextBrowserKit.Options.Export;
using HtmlKit.Model.Tabsheet;

namespace ContextBrowser.Samples.HtmlPages;

public static class HtmlTabsheetTabBase
{
    public static HtmlTabsheetTabInfo EmptyTab(ExportOptions exportOptions)
    {
        return new HtmlTabsheetTabInfo
            (
                info: new TabsheetTabInfo(tabId: "EmptyTab", caption: "Empty"),
                buildHtmlTab: (writer, tabsheetProvider, dto) =>
                {
                    HtmlBuilderFactory.P.Cell(writer, "Empty");
                },
                isActive: false
            )
        { };
    }
}
