using System;
using System.IO;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabInfo<DTO> : IHtmlTabsheetTabInfo<DTO>
{
    public ITabsheetTabInfo Info { get; }
    public Action<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO> BuildHtmlTab { get; set; }
    public bool IsActive { get; set; }

    public HtmlTabsheetTabInfo(ITabsheetTabInfo info, Action<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO> buildHtmlTab, bool isActive)
    {
        Info = info;
        BuildHtmlTab = buildHtmlTab;
        IsActive = isActive;
    }
}