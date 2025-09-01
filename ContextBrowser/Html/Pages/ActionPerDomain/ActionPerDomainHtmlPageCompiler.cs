using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Infrastructure;
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

    public void Compile(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainPage.Build ---", LogLevelNode.None);

        var registrations = new List<IHtmlTabRegistration>
        {
            // Вкладка: Методы
            TabRegistration.For<ActionPerDomainMethodListDataModel, IActionPerDomainMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: true,
                model: new ActionPerDomainMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, $"{dto.DataCell.Action.ToUpper()} -> {dto.DataCell.Domain}");
                    HtmlBuilderFactory.P.Cell(writer, $"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });
                }
            ),

            // Вкладка: Компоненты (PUML)
            TabRegistration.For<ActionPerDomainComponentsDatamodel, IActionPerDomainComponentsDatamodel>(
                tabId: "ComponentsTab",
                caption: "Компоненты",
                isActive: false,
                model: new ActionPerDomainComponentsDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, exportOptions);

                    if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }
            )
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider(registrations);

        var builder = new HtmlPageWithTabsBuilder(contextInfoDataset, exportOptions, tabsheetDataProvider);
        builder.Build((cellData) => $"composite_{cellData.DataCell.Action}_{cellData.DataCell.Domain}.html");
    }
}
