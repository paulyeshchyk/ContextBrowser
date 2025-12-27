using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabInfo<DTO> : IHtmlTabsheetTabInfo<DTO>
{
    public ITabsheetTabInfo Info { get; }

    public Func<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO, CancellationToken, Task> BuildHtmlTab { get; set; }

    public bool IsActive { get; set; }

    public HtmlTabsheetTabInfo(ITabsheetTabInfo info, Func<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO, CancellationToken, Task> buildHtmlTab, bool isActive)
    {
        Info = info;
        BuildHtmlTab = buildHtmlTab;
        IsActive = isActive;
    }
}