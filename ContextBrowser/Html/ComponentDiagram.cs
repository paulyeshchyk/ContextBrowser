using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

public static class PumlComponentDiagramBuilder
{
    public static void Build(IContextInfoDataset model, AppOptions options, IAppLogger<AppLevel> _logger)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ComponentDiagram.Build ---");
        UmlContextComponentDiagram.Build(model.ContextInfoData, options.Export, options.DiagramBuilder);
    }
}
