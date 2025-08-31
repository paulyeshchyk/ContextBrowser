// using System.Reflection;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;
using HtmlKit.Page;

namespace HtmlKit.Page.Compiler;

internal interface IActionPerDomainMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto);
}

internal interface IActionPerDomainComponentsDatamodel : IHtmlTabsheetDataModel
{
    PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}

internal class ActionPerDomainMethodListDataModel : IActionPerDomainMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto)
    {
        return dto.Methods;
    }
}

internal class ActionPerDomainComponentsDatamodel : IActionPerDomainComponentsDatamodel
{
    private readonly PumlHtmlInjectionGeneratorActionAndDomain _pumlInjector = PumlInjector.ClassActionPerDomain;

    public PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        return _pumlInjector(dto.DataCell.Action, dto.DataCell.Domain, exportOptions);
    }
}