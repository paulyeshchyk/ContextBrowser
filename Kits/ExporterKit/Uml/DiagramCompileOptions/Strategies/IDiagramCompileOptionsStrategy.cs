using ContextKit.ContextData.Naming;
using ContextKit.Model;

namespace ExporterKit.Uml.DiagramCompileOptions.Strategies;

public interface IDiagramCompileOptionsStrategy
{
    bool CanHandle(DiagramKind kind);

    IDiagramCompileOptions Create(ILabeledValue meta);
}
