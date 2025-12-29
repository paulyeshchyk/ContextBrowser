using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Builders.Page.Tabs;

public static class HtmlTabsheetBuilder
{
    public static async Task BuildAsync<DTO>(StreamWriter writer, IHtmlTabsheetDataProvider<DTO> tabsDataProvider, DTO cellData, CancellationToken cancellationToken)
    {
        var tabAttributes = new HtmlTagAttributes() { { "class", "tabs" } };

        // Build the tab buttons first
        await HtmlBuilderFactory.Div.WithAsync(writer, tabAttributes, async (token) =>
        {
            foreach (var sheet in tabsDataProvider.Tabsheets)
            {
                var className = sheet.TabInfo.IsActive ? "tab-button active" : "tab-button";
                var attributes = new HtmlTagAttributes() { { "class", className } };
                var cellBuilder = HtmlBuilderFactory.Button.OnClick($"showTab('{sheet.TabInfo.Info.TabId}', this)");
                await cellBuilder.CellAsync(writer, attributes: attributes, sheet.TabInfo.Info.Caption).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);

        // Then build the content for each tab
        foreach (var sheet in tabsDataProvider.Tabsheets)
        {
            var sheetAttributes = new HtmlTagAttributes()
            {
                { "class", sheet.TabInfo.IsActive ? "tab-content active" : "tab-content" },
                { "id", sheet.TabInfo.Info.TabId }
                };

            await HtmlBuilderFactory.Div.WithAsync(writer, sheetAttributes, async (token) =>
            {
                // The ContentGenerator now receives all the data it needs via the DTO.
                await sheet.TabInfo.BuildHtmlTab(writer, tabsDataProvider, cellData, token).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}