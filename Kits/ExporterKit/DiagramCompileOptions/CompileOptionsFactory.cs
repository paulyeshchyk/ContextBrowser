using ContextBrowserKit.Extensions;
using UmlKit.Builders;

namespace ExporterKit.HtmlPageSamples;

public static class CompileOptionsFactory
{
    public static IDiagramCompileOptions ActionStateOptions(string action)
    {
        return new PumlStateCompileOptions
            (
                metaItem: action,
                fetchType: FetchType.FetchAction,
                diagramTitle: $"Action: {action}",
                diagramId: $"state_action_{action}".AlphanumericOnly(),
                outputFileName: $"state_action_{action}.puml"
            );
    }

    public static IDiagramCompileOptions DomainStateCompileOptions(string domain)
    {
        return new PumlStateCompileOptions
            (
                metaItem: domain,
                fetchType: FetchType.FetchDomain,
                diagramTitle: $"Domain: {domain}",
                diagramId: $"state_domain_{domain}".AlphanumericOnly(),
                outputFileName: $"state_domain_{domain}.puml"
            );
    }

    public static IDiagramCompileOptions DomainSequenceCompileOptions(string domain)
    {
        return new PumlSequenceCompileOptions
            (
                metaItem: domain,
                fetchType: FetchType.FetchDomain,
                diagramId: $"sequence_domain_{domain}".AlphanumericOnly(),
                diagramTitle: $"Domain: {domain}",
                outputFileName: $"sequence_domain_{domain}.puml"
            );
    }

    public static IDiagramCompileOptions ActionSequenceCompileOptions(string action)
    {
        return new PumlSequenceCompileOptions
            (
                metaItem: action,
                fetchType: FetchType.FetchAction,
                diagramId: $"sequence_action_{action}".AlphanumericOnly(),
                diagramTitle: $"Action: {action}",
                outputFileName: $"sequence_action_{action}.puml"
            );
    }
}
