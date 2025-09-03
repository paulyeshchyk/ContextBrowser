using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Infrastructure;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

public class ActionPerDomainHtmlPageCompiler : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public ActionPerDomainHtmlPageCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, build

    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var registrations = new List<IHtmlTabRegistration>
        {

            // Вкладка: Классы (PUML)
            TabRegistration.For<IActionPerDomainComponentsDatamodel>(
                tabId: "ClassesTab",
                caption: "Классы",
                isActive: true,
                model: new ActionPerDomainComponentsDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlInjection.Start(writer);
                    pumlInjection.Cell(writer);
                    pumlInjection.End(writer);

                    // if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                    //     HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    // HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }),

            // Вкладка: Методы
            TabRegistration.For<IActionPerDomainMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: false,
                model: new ActionPerDomainMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, innerHtml:$"{dto.DataCell.Action.ToUpper()} -> {dto.DataCell.Domain}");
                    HtmlBuilderFactory.P.Cell(writer,  innerHtml:$"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, innerHtml:method.FullName);
                    });
                }),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider(registrations);

        var builder = new HtmlPageWithTabsBuilder(contextInfoDataset, exportOptions, tabsheetDataProvider);
        builder.Build((cellData) => $"composite_{cellData.DataCell.Action}_{cellData.DataCell.Domain}.html");
    }
}
