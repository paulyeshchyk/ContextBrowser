using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using LoggerKit;
using UmlKit.Builders;
using UmlKit.DiagramGenerator;
using UmlKit.DiagramGenerator.Renderer;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.DiagramCompiler.CoCompiler.State;

public class UmlDiagramCompilerStateAction : UmlDiagramCompilerState
{
    protected override FetchType TypeOfFetch => FetchType.FetchAction;

    public UmlDiagramCompilerStateAction(
        DiagramBuilderOptions options,
        IContextDiagramBuilder builder,
        ExportOptions exportOptions,
        UmlTransitionRendererFlat<UmlState> renderer,
        IAppLogger<AppLevel> logger)
        : base(options, builder, exportOptions, renderer, logger)
    {
    }
}
