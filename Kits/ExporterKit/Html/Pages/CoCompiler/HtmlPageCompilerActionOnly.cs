using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public class HtmlPageCompilerActionOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlPageCompilerActionOnly(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: contextInfo, build, html
    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<ContextKeyContainer>>
        {
            TabsheetFactory.ActionOnlyClassesTabRegistration(exportOptions),
            TabsheetFactory.MethodsTabRegistration(),
            TabsheetFactory.StatesTabRegistration(exportOptions),
            TabsheetFactory.SequenceTabRegistration(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<ContextKeyContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<ContextKeyContainer>(exportOptions, tabsheetDataProvider);
        var builder = new HtmlPageWithTabsEntityListBuilder<ContextKeyContainer>(contextInfoDataset, tabbedPageBuilder, (cellData) => $"composite_action_{cellData.ContextKey.Action}.html");
        builder.Build();
    }
}
