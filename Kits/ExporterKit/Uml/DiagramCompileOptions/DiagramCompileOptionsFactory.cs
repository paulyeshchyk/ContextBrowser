using ContextBrowserKit.Extensions;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public static class DiagramCompileOptionsFactory
{
    public static IDiagramCompileOptions ActionStateOptions(object action)
    {
        var actionStr = (string)action;
        return new PumlStateCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramTitle: $"Action: {action}",
                diagramId: $"state_action_{action}".AlphanumericOnly(),
                outputFileName: $"state_action_{action}.puml");
    }

    public static IDiagramCompileOptions DomainStateCompileOptions(object domain)
    {
        var domainStr = (string)domain;
        return new PumlStateCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramTitle: $"Domain: {domain}",
                diagramId: $"state_domain_{domain}".AlphanumericOnly(),
                outputFileName: $"state_domain_{domain}.puml");
    }

    public static IDiagramCompileOptions DomainSequenceCompileOptions(object domain)
    {
        var domainStr = (string)domain;
        return new PumlSequenceCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramId: $"sequence_domain_{domain}".AlphanumericOnly(),
                diagramTitle: $"Domain: {domain}",
                outputFileName: $"sequence_domain_{domain}.puml");
    }

    public static IDiagramCompileOptions ActionSequenceCompileOptions(object action)
    {
        var actionStr = (string)action;
        return new PumlSequenceCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramId: $"sequence_action_{action}".AlphanumericOnly(),
                diagramTitle: $"Action: {action}",
                outputFileName: $"sequence_action_{action}.puml");
    }
}