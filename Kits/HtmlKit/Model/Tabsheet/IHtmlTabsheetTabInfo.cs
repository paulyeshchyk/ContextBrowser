using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetTabInfo<DTO>
{
    ITabsheetTabInfo Info { get; }

    Func<StreamWriter, IHtmlTabsheetDataProvider<DTO>, DTO, CancellationToken, Task> BuildHtmlTab { get; set; }

    bool IsActive { get; set; }
}
