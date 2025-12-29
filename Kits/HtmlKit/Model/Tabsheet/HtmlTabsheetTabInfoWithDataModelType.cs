using System;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabInfoWithDataModelType<DTO> : IHtmlTabsheetTabInfoWithDataModelType<DTO>
{
    public HtmlTabsheetTabInfo<DTO> TabInfo { get; }

    public Type DataModelType { get; }

    public HtmlTabsheetTabInfoWithDataModelType(HtmlTabsheetTabInfo<DTO> tabInfo, Type dataModelType)
    {
        TabInfo = tabInfo;
        DataModelType = dataModelType;
    }
}
