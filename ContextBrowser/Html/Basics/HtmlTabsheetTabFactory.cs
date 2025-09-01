using ContextBrowserKit.Options.Export;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

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
