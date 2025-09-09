using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace ContextBrowser.Samples.HtmlPages;

public interface IPumlEnbeddedInjectionDatamodel : IHtmlTabsheetDataModel
{
    HtmlBuilder GetEmbeddedPumlInjection(IContextKey contextKey, ExportOptions exportOptions);

    HtmlBuilder GetEmbeddedPumlInjection(string contextKey, ExportOptions exportOptions);
}

public interface IMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer dto);
}