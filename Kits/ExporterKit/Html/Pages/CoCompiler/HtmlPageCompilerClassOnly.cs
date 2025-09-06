using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using ExporterKit.Html.Pages.CoCompiler;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class HtmlPageCompilerClassOnly : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlPageCompilerClassOnly(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: contextInfo, build, html
    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration<EntitynameContainer>>
        {
            TabsheetFactory.ClassesTabRegistration(exportOptions),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider<EntitynameContainer>(registrations);
        var tabbedPageBuilder = new HtmlTabbedPageBuilder<EntitynameContainer>(exportOptions, tabsheetDataProvider);

        var builder = new HtmlPageWithTabsEntityBuilder<EntitynameContainer>(contextInfoDataset, tabbedPageBuilder, (contextInfo) => $"class_only_{contextInfo.AlphanumericOnly()}.html");
        builder.Build();
    }
}