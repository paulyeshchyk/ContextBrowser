using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Puml;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace ContextBrowser.Samples.HtmlPages;

internal class ActionOnlyMethodListDataModel : IActionOnlyMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto)
    {
        return dto.Methods;
    }
}
internal interface IActionOnlyMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto);
}

internal interface IActionOnlyStatesDatamodel : IHtmlTabsheetDataModel
{
    PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}

internal class ActionOnlyStatesDatamodel : IActionOnlyStatesDatamodel
{
    private readonly PumlHtmlInjectionGeneratorAction _pumlInjector = PumlInjector.InjectActionStateEmbeddedHtml;

    public PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        return _pumlInjector(dto.DataCell.Action, exportOptions);
    }
}

internal interface IActionOnlySequenceDatamodel : IHtmlTabsheetDataModel
{
    PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}

internal class ActionOnlySequenceDatamodel : IActionOnlySequenceDatamodel
{
    private readonly PumlHtmlInjectionGeneratorAction _pumlInjector = PumlInjector.InjectActionSequenceEmbeddedHtml;

    public PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        return _pumlInjector(dto.DataCell.Action, exportOptions);
    }
}