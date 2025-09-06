using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Services;

public interface IUmlDiagramCompilerOrchestrator
{
    void CompileAll(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions builderOptions);
}
