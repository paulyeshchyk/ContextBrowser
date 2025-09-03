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
    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ActionOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration>
        {

            // Вкладка: Классы (PUML)
            TabRegistration.For<IActionOnlyClassesDatamodel>(
                tabId: "ClassesTab",
                caption: "Классы",
                isActive: true,
                model: new ActionOnlyClassesDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlBuilder = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlBuilder.Start(writer);
                    pumlBuilder.Cell(writer);
                    pumlBuilder.End(writer);
                }),

            // Вкладка: Методы
            TabRegistration.For<IActionOnlyMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: false,
                model: new ActionOnlyMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, innerHtml:$"{dto.DataCell.Action.ToUpper()}");
                    HtmlBuilderFactory.P.Cell(writer, innerHtml:$"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, innerHtml:method.FullName);
                    });
                }),

            // Вкладка: Состояния (PUML)
            TabRegistration.For<IActionOnlyStatesDatamodel>(
                tabId: "StatesTab",
                caption: "Состояния",
                isActive: false,
                model: new ActionOnlyStatesDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlBuilder = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlBuilder.Start(writer);
                    pumlBuilder.Cell(writer);
                    pumlBuilder.End(writer);
                }),

            // Вкладка: Компоненты (PUML)
            TabRegistration.For<IActionOnlySequenceDatamodel>(
                tabId: "SequenceTab",
                caption: "Последовательности",
                isActive: false,
                model: new ActionOnlySequenceDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlBuilder = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlBuilder.Start(writer);
                    pumlBuilder.Cell(writer);
                    pumlBuilder.End(writer);
                }),
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider(registrations);

        var builder = new HtmlPageWithTabsBuilder(contextInfoDataset, exportOptions, tabsheetDataProvider);
        builder.Build((cellData) => $"composite_action_{cellData.DataCell.Action}.html");
    }
}