using ContextBrowser.Html.Composite;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using LoggerKit;

public static partial class ActionPerDomainHtmlPageBuilder
{
    // context: html, build
    public static void Build(IContextInfoDataset contextInfoDataset, AppOptions appOptions, IAppLogger<AppLevel> _logger)
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
                    var pumlInjection = model.GetEmbeddedPumlInjection(dto, appOptions.Export);

                    if (!string.IsNullOrWhiteSpace(pumlInjection.EmbeddedScript))
                        HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedScript);

                    HtmlBuilderFactory.Raw.Cell(writer, pumlInjection.EmbeddedContent);
                }
            )
        };

        var tabsheetDataProvider = new ComposableTabsheetDataProvider(registrations);

        var builder = new HtmlPageWithTabsBuilder(contextInfoDataset, appOptions.Export, tabsheetDataProvider);
        builder.Build((cellData) => $"composite_{cellData.DataCell.Action}_{cellData.DataCell.Domain}.html");
    }
}
