using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Infrastructure;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace HtmlKit.Page.Compiler;

// context: contextInfo, build, html
public class DomainOnlyHtmlPageCompiler : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public DomainOnlyHtmlPageCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: contextInfo, build, html
    public void Compile(IContextInfoDataset contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration>
        {
            // Вкладка: Классы (PUML)
            TabRegistration.For<IDomainOnlyClassesDatamodel>(
                tabId: "ClassesTab",
                caption: "Классы",
                isActive: true,
                model: new DomainOnlyClassesDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlBuilder = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlBuilder.Start(writer);
                    pumlBuilder.Cell(writer);
                    pumlBuilder.End(writer);
                }),

            // Вкладка: Методы
            TabRegistration.For<IDomainOnlyMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: false,
                model: new DomainOnlyMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, innerHtml:$"{dto.DataCell.Domain.ToUpper()}");
                    HtmlBuilderFactory.P.Cell(writer, innerHtml:$"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, innerHtml:method.FullName);
                    });
                }),

            // Вкладка: Компоненты (PUML)
            TabRegistration.For<IDomainOnlySequenceDatamodel>(
                tabId: "SequenceTab",
                caption: "Последовательности",
                isActive: false,
                model: new DomainOnlySequenceDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlBuilder = model.GetEmbeddedPumlInjection(dto, exportOptions);
                    pumlBuilder.Start(writer);
                    pumlBuilder.Cell(writer);
                    pumlBuilder.End(writer);
                }),

            // Вкладка: Состояния (PUML)
            TabRegistration.For<IDomainOnlyStatesDatamodel>(
                tabId: "StatesTab",
                caption: "Состояния",
                isActive: false,
                model: new DomainOnlyStatesDatamodel(),
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
        builder.Build((cellData) => $"composite_domain_{cellData.DataCell.Domain}.html");
    }
}