using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public static class DiagramCompileOptionsFactory
{
    public static IDiagramCompileOptions ActionStateOptions(ILabeledValue action, INamingProcessor namingProcessor)
    {
        var actionStr = (string)action.LabeledData;
        return new PumlStateCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramTitle: $"Action: {action}",
                diagramId: namingProcessor.StateActionDiagramId(action.LabeledData.ToString()),
                outputFileName: namingProcessor.StateActionPumlFilename(action.LabeledData.ToString()));
    }

    public static IDiagramCompileOptions DomainStateCompileOptions(ILabeledValue domain, INamingProcessor namingProcessor)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlStateCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramTitle: $"Domain: {domain}",
                diagramId: namingProcessor.StateDomainDiagramId(domain.LabeledData.ToString()),
                outputFileName: namingProcessor.StateDomainPumlFilename(domain.LabeledData.ToString()));
    }

    public static IDiagramCompileOptions DomainSequenceCompileOptions(ILabeledValue domain, INamingProcessor namingProcessor)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramId: namingProcessor.SequenceDomainDiagramId(domain.LabeledData.ToString()),
                diagramTitle: $"Domain: {domain}",
                outputFileName: namingProcessor.SequenceDomainPumlFilename(domain.LabeledData.ToString()));
    }

    public static IDiagramCompileOptions ActionSequenceCompileOptions(ILabeledValue action, INamingProcessor namingProcessor)
    {
        var actionStr = (string)action.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramId: namingProcessor.SequenceActionDiagramId(action.LabeledData.ToString()),
                diagramTitle: $"Action: {action}",
                outputFileName: namingProcessor.SequenceActionPumlFilename(action.LabeledData.ToString()));
    }
}