using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using TensorKit.Model;

namespace ContextBrowser.Samples.HtmlPages;

public interface IPumlEnbeddedInjectionDatamodel<TTensor> : IHtmlTabsheetDataModel
    where TTensor : notnull
{
    HtmlBuilder GetPumlBuilder(TTensor contextKey, ExportOptions exportOptions);

    HtmlBuilder GetPumlBuilder(string contextKey, ExportOptions exportOptions);
}

public interface IMethodListDatamodel<TTensor> : IHtmlTabsheetDataModel
    where TTensor : ITensor<string>
{
    IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<TTensor> dto);
}