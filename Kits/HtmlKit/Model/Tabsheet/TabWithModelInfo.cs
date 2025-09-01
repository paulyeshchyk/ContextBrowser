using System;
using HtmlKit.Model;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabInfoWithDataModelType
{
    public HtmlTabsheetTabInfo TabInfo { get; }
    public Type DataModelType { get; }

    public HtmlTabsheetTabInfoWithDataModelType(HtmlTabsheetTabInfo tabInfo, Type dataModelType)
    {
        TabInfo = tabInfo;
        DataModelType = dataModelType;
    }
}
