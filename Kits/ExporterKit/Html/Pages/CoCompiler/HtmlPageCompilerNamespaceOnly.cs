using System.Collections.Generic;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Model;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class HtmlPageCompilerNamespaceOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlPageCompilerNamespaceOnly(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: contextInfo, build, html
    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- NamespaceOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<NamespacenameContainer>>
        {
            TabsheetFactory.NamespaceTabRegistration(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<NamespacenameContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<NamespacenameContainer>(exportOptions, tabsheetDataProvider);

        var builder = new HtmlPageWithTabsNamespaceEntityBuilder<NamespacenameContainer>(contextInfoDataset, tabbedPageBuilder, (ns) => $"namespace_only_{ns.AlphanumericOnly()}.html");
        builder.Build();
    }
}