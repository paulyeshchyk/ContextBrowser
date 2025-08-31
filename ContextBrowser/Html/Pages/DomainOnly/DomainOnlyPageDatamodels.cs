using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Puml;
using HtmlKit.Model;
using HtmlKit.Model.Tabsheet;

namespace HtmlKit.Page.Compiler;

internal class DomainOnlyMethodListDataModel : IDomainOnlyMethodListDatamodel
{
    public IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto)
    {
        return dto.Methods;
    }
}
internal interface IDomainOnlyMethodListDatamodel : IHtmlTabsheetDataModel
{
    IEnumerable<ContextInfo> GetMethodsList(HtmlContextInfoDataCell dto);
}

internal interface IDomainOnlyStatesDatamodel : IHtmlTabsheetDataModel
{
    PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}

internal class DomainOnlyStatesDatamodel : IDomainOnlyStatesDatamodel
{
    private readonly PumlHtmlInjectionGeneratorDomain _pumlInjector = PumlInjector.InjectDomainStateEmbeddedHtml;

    public PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        return _pumlInjector(dto.DataCell.Domain, exportOptions);
    }
}

internal interface IDomainOnlySequenceDatamodel : IHtmlTabsheetDataModel
{
    PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions);
}

internal class DomainOnlySequenceDatamodel : IDomainOnlySequenceDatamodel
{
    private readonly PumlHtmlInjectionGeneratorDomain _pumlInjector = PumlInjector.InjectDomainSequenceEmbeddedHtml;

    public PumlHtmlInjection GetEmbeddedPumlInjection(HtmlContextInfoDataCell dto, ExportOptions exportOptions)
    {
        return _pumlInjector(dto.DataCell.Domain, exportOptions);
    }
}