using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;

namespace ContextBrowser.Samples.HtmlPages;

internal interface IPumlEnbeddedInjectionDatamodel : IHtmlTabsheetDataModel
{
    HtmlBuilder GetEmbeddedPumlInjection(IContextKey contextKey, ExportOptions exportOptions);
    HtmlBuilder GetEmbeddedPumlInjection(string contextKey, ExportOptions exportOptions);
}

internal interface IMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer dto);
}