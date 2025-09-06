using System;
using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace HtmlKit.Model.Tabsheet;

public interface IHtmlTabsheetDataProvider<DTO> : IHtmlDataProvider
{
    IEnumerable<HtmlTabsheetTabInfoWithDataModelType<DTO>> Tabsheets { get; }

    T GetTabsheetDataModel<T>() where T : IHtmlTabsheetDataModel;
}

public interface IHtmlTabsheetDataModel
{

}