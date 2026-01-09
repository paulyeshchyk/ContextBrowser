using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions.Strategies;

public class ActionStateCompileOptionsStrategy : IDiagramCompileOptionsStrategy
{
    private readonly INamingProcessor _namingProcessor;

    public ActionStateCompileOptionsStrategy(INamingProcessor namingProcessor) => _namingProcessor = namingProcessor;

    public bool CanHandle(DiagramKind kind) => kind == DiagramKind.ActionState;

    public IDiagramCompileOptions Create(ILabeledValue meta)
    {
        var actionStr = (string)meta.LabeledData;
        return new PumlStateCompileOptions
        (
            metaItem: actionStr,
            fetchType: FetchType.FetchAction,
            diagramTitle: $"Action: {meta}",
            diagramId: _namingProcessor.StateActionDiagramId(meta.LabeledData.ToString()),
            outputFileName: _namingProcessor.StateActionPumlFilename(meta.LabeledData.ToString())
        );
    }
}
