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
        // Build the tab buttons first
        HtmlBuilderFactory.Div.With(writer, () =>
        {
            foreach (var sheet in tabsDataProvider.Tabsheets)
            {
                var className = sheet.TabInfo.IsActive ? "tab-button active" : "tab-button";
                HtmlBuilderFactory.Button.OnClick($"showTab('{sheet.TabInfo.Info.TabId}', this)").Cell(writer, sheet.TabInfo.Info.Caption, className: className);
            }
        }, className: "tabs");

        // Then build the content for each tab
        foreach (var sheet in tabsDataProvider.Tabsheets)
        {
            var className = sheet.TabInfo.IsActive ? "tab-content active" : "tab-content";
            HtmlBuilderFactory.Div.With(writer, () =>
            {
                // The ContentGenerator now receives all the data it needs via the DTO.
                sheet.TabInfo.BuildHtmlTab(writer, tabsDataProvider, cellData);
            }, className: className, id: sheet.TabInfo.Info.TabId);
        }
    }
}
