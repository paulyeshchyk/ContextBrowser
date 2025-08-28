using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetDataProvider : IHtmlDataProvider
{
    IEnumerable<HtmlTabsheetTabInfoWithDataModelType> Tabsheets { get; }

    T GetTabsheetDataModel<T>() where T : IHtmlTabsheetDataModel;
}

public interface IHtmlTabsheetDataModel
{

}