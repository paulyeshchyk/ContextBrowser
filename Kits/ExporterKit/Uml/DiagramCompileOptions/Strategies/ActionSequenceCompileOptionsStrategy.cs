using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Uml.DiagramCompileOptions.Strategies;
using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions.Strategies;

public class ActionSequenceCompileOptionsStrategy : IDiagramCompileOptionsStrategy
{
    private readonly INamingProcessor _namingProcessor;

    public ActionSequenceCompileOptionsStrategy(INamingProcessor namingProcessor) => _namingProcessor = namingProcessor;


    public bool CanHandle(DiagramKind kind) => kind == DiagramKind.ActionSequence;

    public IDiagramCompileOptions Create(ILabeledValue action)
    {
        var actionStr = (string)action.LabeledData;
        return new PumlSequenceCompileOptions
            (
                metaItem: actionStr,
                fetchType: FetchType.FetchAction,
                diagramId: _namingProcessor.SequenceActionDiagramId(action.LabeledData.ToString()),
                diagramTitle: $"Action: {action}",
                outputFileName: _namingProcessor.SequenceActionPumlFilename(action.LabeledData.ToString()));
    }
}
