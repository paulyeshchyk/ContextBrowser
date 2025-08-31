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
using LoggerKit;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public static class DomainOnlyHtmlPageBuilder
{
    // context: contextInfo, build, html
    public static void Build(IContextInfoDataset contextInfoDataset, AppOptions appOptions, IAppLogger<AppLevel> _logger)
    {
        _logger.WriteLogObject(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- DomainOnly.Build ---", LogLevelNode.None));

        var registrations = new List<IHtmlTabRegistration>
        {
            // Вкладка: Методы
            TabRegistration.For<DomainOnlyMethodListDataModel, IDomainOnlyMethodListDatamodel>(
                tabId: "MethodsTab",
                caption: "Методы",
                isActive: true,
                model: new DomainOnlyMethodListDataModel(),
                build: (writer, model, dto) =>
                {
                    var methods = model.GetMethodsList(dto);

                    HtmlBuilderFactory.H1.Cell(writer, $"{dto.DataCell.Domain.ToUpper()}");
                    HtmlBuilderFactory.P.Cell(writer, $"Methods: {methods.Count()}");

                    HtmlBuilderFactory.Ul.With(writer, () =>
                    {
                        foreach (var method in methods.Distinct())
                            HtmlBuilderFactory.Li.Cell(writer, method.FullName);
                    });
                }
            ),

            // Вкладка: Состояния (PUML)
            TabRegistration.For<DomainOnlyStatesDatamodel, IDomainOnlyStatesDatamodel>(
                tabId: "StatesTab",
                caption: "Состояния",
                isActive: false,
                model: new DomainOnlyStatesDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, appOptions.Export);

                    if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }
            ),

            // Вкладка: Компоненты (PUML)
            TabRegistration.For<DomainOnlySequenceDatamodel, IDomainOnlySequenceDatamodel>(
                tabId: "SequenceTab",
                caption: "Последовательности",
                isActive: false,
                model: new DomainOnlySequenceDatamodel(),
                build: (writer, model, dto) =>
                {
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, appOptions.Export);

                    if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }
            )
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider(registrations);

        var builder = new HtmlPageWithTabsBuilder(contextInfoDataset, appOptions.Export, tabsheetDataProvider);
        builder.Build((cellData) => $"composite_domain_{cellData.DataCell.Domain}.html");
    }
}