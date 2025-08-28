using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

public static class ComponentDiagram
{
    public static void Build(IContextInfoDataset model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Cntx, "--- ComponentDiagram.Build ---");
        UmlContextComponentDiagram.Build(model.ContextInfoData, options.Export, options.DiagramBuilder);
    }
}
