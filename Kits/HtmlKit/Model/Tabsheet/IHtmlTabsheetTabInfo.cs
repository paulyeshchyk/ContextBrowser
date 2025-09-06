using System;
using System.IO;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetTabInfo<DTO>
{
    ITabsheetTabInfo Info { get; }
    Action<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO> BuildHtmlTab { get; set; }
    bool IsActive { get; set; }
}
