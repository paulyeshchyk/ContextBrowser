using System.Collections.Generic;
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
    HtmlBuilder GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}
internal interface IMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto);
}

internal interface IActionOnlyClassesDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class ActionOnlyClassesDatamodel : PumlTabsheetDatamodel, IActionOnlyClassesDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"class_action_{dto.DataCell.Action}.puml";
}

internal interface IActionOnlyMethodListDatamodel : IMethodListDatamodel
{
}

internal class ActionOnlyMethodListDataModel : IActionOnlyMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto) => dto.Methods;
}

internal interface IActionOnlySequenceDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class ActionOnlySequenceDatamodel : PumlTabsheetDatamodel, IActionOnlySequenceDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"sequence_action_{dto.DataCell.Action}.puml";
}

internal interface IActionOnlyStatesDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class ActionOnlyStatesDatamodel : PumlTabsheetDatamodel, IActionOnlyStatesDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"state_action_{dto.DataCell.Action}.puml";
}

internal interface IActionPerDomainComponentsDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class ActionPerDomainComponentsDatamodel : PumlTabsheetDatamodel, IActionPerDomainComponentsDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"class_{dto.DataCell.Action}_{dto.DataCell.Domain}.puml";
}

internal interface IActionPerDomainMethodListDatamodel : IMethodListDatamodel
{
}

internal class ActionPerDomainMethodListDataModel : IActionPerDomainMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto) => dto.Methods;
}

internal interface IDomainOnlyClassesDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class DomainOnlyClassesDatamodel : PumlTabsheetDatamodel, IDomainOnlyClassesDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"class_domain_{dto.DataCell.Domain}.puml";
}

internal interface IDomainOnlyMethodListDatamodel : IMethodListDatamodel
{
}

internal class DomainOnlyMethodListDataModel : IDomainOnlyMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto) => dto.Methods;
}

internal interface IDomainOnlySequenceDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class DomainOnlySequenceDatamodel : PumlTabsheetDatamodel, IDomainOnlySequenceDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"sequence_domain_{dto.DataCell.Domain}.puml";
}

internal interface IDomainOnlyStatesDatamodel : IPumlEnbeddedInjectionDatamodel
{
}

internal class DomainOnlyStatesDatamodel : PumlTabsheetDatamodel, IDomainOnlyStatesDatamodel
{
    protected override string GetPumlFileName(HtmlContextInfoDataCell dto) => $"state_domain_{dto.DataCell.Domain}.puml";
}
