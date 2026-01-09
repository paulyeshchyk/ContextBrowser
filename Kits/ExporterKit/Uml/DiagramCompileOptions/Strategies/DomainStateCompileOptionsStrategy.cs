using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions.Strategies;

public class DomainStateCompileOptionsStrategy : IDiagramCompileOptionsStrategy
{
    private readonly INamingProcessor _namingProcessor;

    public DomainStateCompileOptionsStrategy(INamingProcessor namingProcessor) => _namingProcessor = namingProcessor;

    public bool CanHandle(DiagramKind kind) => kind == DiagramKind.DomainState;

    public IDiagramCompileOptions Create(ILabeledValue domain)
    {
        var domainStr = (string)domain.LabeledData;
        return new PumlStateCompileOptions
            (
                metaItem: domainStr,
                fetchType: FetchType.FetchDomain,
                diagramTitle: $"Domain: {domain}",
                diagramId: _namingProcessor.StateDomainDiagramId(domain.LabeledData.ToString()),
                outputFileName: _namingProcessor.StateDomainPumlFilename(domain.LabeledData.ToString()));
    }
}
