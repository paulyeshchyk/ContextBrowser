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

public interface IPumlEnbeddedInjectionDatamodel<TKey> : IHtmlTabsheetDataModel
    where TKey : TensorBase<string>
{
    HtmlBuilder GetPumlBuilder(TKey contextKey, ExportOptions exportOptions);

    HtmlBuilder GetPumlBuilder(string contextKey, ExportOptions exportOptions);
}

public interface IMethodListDatamodel<TKey> : IHtmlTabsheetDataModel
    where TKey : TensorBase<string>
{
    IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<TKey> dto);
}