using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions.Strategies;

public class DomainSequenceCompileOptionsStrategy : IDiagramCompileOptionsStrategy
{
    private readonly INamingProcessor _namingProcessor;

    public DomainSequenceCompileOptionsStrategy(INamingProcessor namingProcessor) => _namingProcessor = namingProcessor;

    public bool CanHandle(DiagramKind kind) => kind == DiagramKind.DomainSequence;

    public IDiagramCompileOptions Create(ILabeledValue domain)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramId: _namingProcessor.SequenceDomainDiagramId(domain.LabeledData.ToString()),
                diagramTitle: $"Domain: {domain}",
                outputFileName: _namingProcessor.SequenceDomainPumlFilename(domain.LabeledData.ToString()));
    }
}
