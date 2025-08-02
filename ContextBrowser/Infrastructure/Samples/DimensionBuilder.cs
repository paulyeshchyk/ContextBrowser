using ContextBrowser.DiagramFactory;
using ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders;
using ContextBrowser.ExporterKit;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Diagrams;

namespace ContextBrowser.Infrastructure.Samples;

// context: contextInfo, build, html
public static class DimensionBuilder
{
    // context: contextInfo, build, html
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- DimensionBuilder.Build ---");
        var callback = AdaptToDomainCallback(
            contextItems: model.contextsList,
            classifier: new ContextClassifier(),
            outputPath: options.outputDirectory,
            onWriteLog: onWriteLog,
            factory:(owl) => ContextDiagramFactory.Custom(options.diagramType, options.contextTransitionDiagramBuilderOptions, owl)
        );

        var builder = new HtmlContextDimensionBuilder(
            model.matrix,
            model.contextsList,
            options.outputDirectory,
            callback,
            () => ContextDiagramFactory.Transition(options.contextTransitionDiagramBuilderOptions, onWriteLog));

        builder.Build();
    }

    // context: contextInfo, build, html
    internal static Func<string, bool> AdaptToDomainCallback(List<ContextInfo> contextItems, ContextClassifier classifier, string outputPath, OnWriteLog? onWriteLog, Func<OnWriteLog?, IContextDiagramBuilder> factory)
    {
        return domain =>
        {
            var diagram = new UmlDiagramSequence();
            diagram.SetTitle($"Domain: {domain}");
            diagram.SetSkinParam("componentStyle", "rectangle");

            var builder = factory(onWriteLog);
            var success = builder.Build(domain, contextItems, classifier, diagram);

            if(success)
            {
                var path = Path.Combine(outputPath, $"sequence_domain_{domain}.puml");
                diagram.WriteToFile(path);
            }

            return success;
        };
    }
}