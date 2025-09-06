using System;
using System.IO;

namespace HtmlKit.Model.Tabsheet;

public record TabsheetTabInfo : ITabsheetTabInfo
{
    public string TabId { get; }
    public string Caption { get; }

    public TabsheetTabInfo(string tabId, string caption)
    {
        TabId = tabId;
        Caption = caption;
    }
}
