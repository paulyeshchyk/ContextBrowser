using ContextBrowserKit.Extensions;
using ContextKit.Model;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public static class DiagramCompileOptionsFactory
{
    public static IDiagramCompileOptions ActionStateOptions(ILabeledValue action)
    {
        var actionStr = (string)action.LabeledData;
        return new PumlStateCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramTitle: $"Action: {action}",
                diagramId: $"state_action_{action}".AlphanumericOnly(),
                outputFileName: $"state_action_{action}.puml");
    }

    public static IDiagramCompileOptions DomainStateCompileOptions(ILabeledValue domain)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlStateCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramTitle: $"Domain: {domain}",
                diagramId: $"state_domain_{domain}".AlphanumericOnly(),
                outputFileName: $"state_domain_{domain}.puml");
    }

    public static IDiagramCompileOptions DomainSequenceCompileOptions(ILabeledValue domain)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramId: $"sequence_domain_{domain}".AlphanumericOnly(),
                diagramTitle: $"Domain: {domain}",
                outputFileName: $"sequence_domain_{domain}.puml");
    }

    public static IDiagramCompileOptions ActionSequenceCompileOptions(ILabeledValue action)
    {
        var actionStr = (string)action.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramId: $"sequence_action_{action}".AlphanumericOnly(),
                diagramTitle: $"Action: {action}",
                outputFileName: $"sequence_action_{action}.puml");
    }
}