using System.Collections.Generic;
using System.IO;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace HtmlKit.Builders.Page;

public static class HtmlTabsheetBuilder
{
    public static void Build(StreamWriter writer, IHtmlTabsheetDataProvider tabsDataProvider, HtmlContextInfoDataCell cellData)
    {
        var tabAttributes = new HtmlTagAttributes() { { "class", "tabs" } };
        // Build the tab buttons first
        HtmlBuilderFactory.Div.With(writer, tabAttributes, () =>
        {
            foreach (var sheet in tabsDataProvider.Tabsheets)
            {
                var className = sheet.TabInfo.IsActive ? "tab-button active" : "tab-button";
                var attributes = new HtmlTagAttributes() { { "class", className } };
                HtmlBuilderFactory.Button.OnClick($"showTab('{sheet.TabInfo.Info.TabId}', this)").Cell(writer, attributes: attributes, sheet.TabInfo.Info.Caption);
            }
        });

        // Then build the content for each tab
        foreach (var sheet in tabsDataProvider.Tabsheets)
        {
            var sheetAttributes = new HtmlTagAttributes()
            {
                { "class", sheet.TabInfo.IsActive ? "tab-content active" : "tab-content" },
                { "id", sheet.TabInfo.Info.TabId }
                };

            HtmlBuilderFactory.Div.With(writer, sheetAttributes, () =>
                {
                    // The ContentGenerator now receives all the data it needs via the DTO.
                    sheet.TabInfo.BuildHtmlTab(writer, tabsDataProvider, cellData);
                });
        }
    }
}
