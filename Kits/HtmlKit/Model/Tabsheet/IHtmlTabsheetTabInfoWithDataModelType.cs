using System;
using HtmlKit.Model;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetTabInfoWithDataModelType<DTO>
{
    HtmlTabsheetTabInfo<DTO> TabInfo { get; }
    Type DataModelType { get; }
}
