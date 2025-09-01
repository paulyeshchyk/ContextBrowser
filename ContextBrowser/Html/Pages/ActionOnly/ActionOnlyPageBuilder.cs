using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Html;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public class ActionOnlyHtmlPageCompiler : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public ActionOnlyHtmlPageCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: contextInfo, build, html

    public void Compile(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration>
        {
            // Вкладка: Методы
            TabRegistration.For<ActionOnlyMethodListDataModel, IActionOnlyMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: true,
                model: new ActionOnlyMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, $"{dto.DataCell.Action.ToUpper()}");
                    HtmlBuilderFactory.P.Cell(writer, $"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });
                }
            ),

            // Вкладка: Состояния (PUML)
            TabRegistration.For<ActionOnlyStatesDatamodel, IActionOnlyStatesDatamodel>(
                tabId: "StatesTab",
                caption: "Состояния",
                isActive: false,
                model: new ActionOnlyStatesDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, exportOptions);

                    if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }
            ),

            // Вкладка: Компоненты (PUML)
            TabRegistration.For<ActionOnlySequenceDatamodel, IActionOnlySequenceDatamodel>(
                tabId: "SequenceTab",
                caption: "Последовательности",
                isActive: false,
                model: new ActionOnlySequenceDatamodel(),
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
        builder.Build((cellData) => $"composite_action_{cellData.DataCell.Action}.html");
    }
}