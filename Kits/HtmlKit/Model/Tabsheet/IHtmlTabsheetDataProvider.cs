using System.Collections.Generic;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetDataProvider<DTO> : IHtmlDataProvider
{
    IEnumerable<IHtmlTabsheetTabInfoWithDataModelType<DTO>> Tabsheets { get; }

    T GetTabsheetDataModel<T>() where T : IHtmlTabsheetDataModel;
}

public interface IHtmlTabsheetDataModel
{
}