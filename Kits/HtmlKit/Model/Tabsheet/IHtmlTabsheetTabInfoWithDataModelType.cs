using System;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetTabInfoWithDataModelType<DTO>
{
    HtmlTabsheetTabInfo<DTO> TabInfo { get; }

    Type DataModelType { get; }
}
