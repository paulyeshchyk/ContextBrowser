namespace HtmlKit.Model.Tabsheet;

public record TabsheetTabInfo
{
    public readonly string TabId;
    public readonly string Caption;

    public TabsheetTabInfo(string tabId, string caption)
    {
        TabId = tabId;
        Caption = caption;
    }
}

public class HtmlTabsheetTabInfo
{
    public TabsheetTabInfo Info { get; }
    public Action<StreamWriter, IHtmlTabsheetDataProvider, HtmlContextInfoDataCell> BuildHtmlTab { get; set; }
    public bool IsActive { get; set; }

    public HtmlTabsheetTabInfo(TabsheetTabInfo info, Action<StreamWriter, IHtmlTabsheetDataProvider, HtmlContextInfoDataCell> buildHtmlTab, bool isActive)
    {
        Info = info;
        BuildHtmlTab = buildHtmlTab;
        IsActive = isActive;
    }
}
