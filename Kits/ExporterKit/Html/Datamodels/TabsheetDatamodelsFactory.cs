using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Model.Containers;
using HtmlKit.Model.Tabsheet;

namespace ExporterKit.Html.Datamodels;

public interface IPumlEnbeddedInjectionDatamodel<TTensor> : IHtmlTabsheetDataModel
    where TTensor : notnull
{
    HtmlBuilder GetPumlBuilder(TTensor contextKey, ExportOptions exportOptions);

    HtmlBuilder GetPumlBuilder(string contextKey, ExportOptions exportOptions);
}

public interface IMethodListDatamodel<TTensor> : IHtmlTabsheetDataModel
    where TTensor : notnull
{
    IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto);
}