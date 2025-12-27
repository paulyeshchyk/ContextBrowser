using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using ExporterKit.Html.Datamodels;
using HtmlKit.Builders.Page;
using HtmlKit.Builders.Page.Tabs;
using HtmlKit.Document;
using HtmlKit.Matrix;
using HtmlKit.Model.Containers;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler;

public static class TabsheetFactory<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string HtmlPathTemplateCompositeDomain = "/pages/composite_domain_{0}.html";
    private const string HtmlPathTemplateCompositeAction = "/pages/composite_action_{0}.html";

    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> DomainMindmapTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Mindmap (PUML)
        return TabRegistration.For<ClassOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerEntityName>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new ClassOnlyMindmapDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> DomainClassesTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ClassOnlyDatamodel<TDataTensor>, ContextInfoKeyContainerEntityName>(
            tabId: "ClassesTab",
            caption: "Общая",
            isActive: true,
            model: new ClassOnlyDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerNamespace> NamespaceTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<NamespaceOnlyDatamodel<TDataTensor>, ContextInfoKeyContainerNamespace>(
            tabId: "NamespacesTab",
            caption: "Общая",
            isActive: true,
            model: new NamespaceOnlyDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionOnlyClassesTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionOnlyClassesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionOnlyClassesDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> MethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory, INamingProcessor namingProcessor)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionOnlyMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionOnlyMethodListDataModel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var titleAction = (string)dto.ContextKey.Action;
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: $"Action: <b>{titleAction}</b>", isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: "-", cancellationToken: token).ConfigureAwait(false);

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                await matrixWriter.WriteAsync(writer, methodsMatrix, null, options, token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> StatesTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyStatesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new ActionOnlyStatesDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> SequenceTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<ActionOnlySequenceDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new ActionOnlySequenceDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory, INamingProcessor namingProcessor)
    {
        // Вкладка: Методы
        return TabRegistration.For<SummaryMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new SummaryMethodListDataModel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: $"Domain: <b>{titleDomain}</b>", isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: "-", cancellationToken: token).ConfigureAwait(false);

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                await matrixWriter.WriteAsync(writer, methodsMatrix, null, options, token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyClassesTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainSummaryComponentsDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainSummaryComponentsDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionOnlyMindmap(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new ActionOnlyMindmapDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMindmap(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new DomainOnlyMindmapDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyStates(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyStatesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new DomainOnlyStatesDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlySequence(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<DomainOnlySequenceDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new DomainOnlySequenceDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMethodsTabsheetRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory, INamingProcessor namingProcessor)
    {
        // Вкладка: Методы
        return TabRegistration.For<DomainOnlyMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new DomainOnlyMethodListDataModel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: $"Domain: <b>{titleDomain}</b>", isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: "-", cancellationToken: token).ConfigureAwait(false);

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                await matrixWriter.WriteAsync(writer, methodsMatrix, null, options, token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyClassesTabsheetRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainOnlyClassesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainOnlyClassesDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlBuilder.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlBuilder.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionPerDomainMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory, INamingProcessor namingProcessor)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainMethodListDataModel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                var linkDomain = string.Format(HtmlPathTemplateCompositeDomain, titleDomain);
                var titleAction = (string)dto.ContextKey.Action;
                var linkAction = string.Format(HtmlPathTemplateCompositeAction, titleAction);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: $"Domain: <a href=\"{linkDomain}\">{titleDomain}</a>", isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: $"Action: <a href=\"{linkAction}\">{titleAction}</a>", isEncodable: false, cancellationToken: token).ConfigureAwait(false);
                await HtmlBuilderFactory.Div.CellAsync(writer, innerHtml: "-", cancellationToken: token).ConfigureAwait(false);

                var methodIndexList = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodIndexList, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                await matrixWriter.WriteAsync(writer, methodsMatrix, null, options, token).ConfigureAwait(false);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionPerDomainClassesTabRegistration(ExportOptions exportOptions, INamingProcessor namingProcessor)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionSummaryDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionSummaryDatamodel<TDataTensor>(namingProcessor),
            build: async (writer, model, dto, token) =>
            {
                var pumlInjection = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                await pumlInjection.StartAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlInjection.CellAsync(writer, cancellationToken: token).ConfigureAwait(false);
                await pumlInjection.EndAsync(writer, cancellationToken: token).ConfigureAwait(false);
            });
    }
}

internal class SummaryMethodListDataModel<TDataTensor> : IMethodListDatamodel<TDataTensor>
    where TDataTensor : notnull
{
    private readonly INamingProcessor _namingProcessor;

    public SummaryMethodListDataModel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TDataTensor> dto) => dto.ContextInfoList;
}

internal class ActionPerDomainMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    private readonly INamingProcessor _namingProcessor;

    public ActionPerDomainMethodListDataModel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class DomainOnlyMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    private readonly INamingProcessor _namingProcessor;

    public DomainOnlyMethodListDataModel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class ActionOnlyMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    private readonly INamingProcessor _namingProcessor;

    public ActionOnlyMethodListDataModel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class NamespaceOnlyDatamodel<TTensor> : PumlEmbeddedContentDatamodel<TTensor>, IPumlEnbeddedInjectionDatamodel<TTensor>
    where TTensor : notnull
{
    private const string PumlFilenameTemplate = "namespace_only_{0}.puml";
    private readonly INamingProcessor _namingProcessor;

    public NamespaceOnlyDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    protected override string GetPumlFileName(TTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => string.Format(PumlFilenameTemplate, contextKey.AlphanumericOnly());
}

internal class DomainSummaryComponentsDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public DomainSummaryComponentsDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.ClassActionDomainPumlFilename(contextKey.Action, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyStatesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "state_domain_{0}.puml";
    private readonly INamingProcessor _namingProcessor;

    public DomainOnlyStatesDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public ActionOnlyMindmapDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.MindmapActionPumlFilename(contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public DomainOnlyMindmapDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.MindmapDomainPumlFilename(contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlySequenceDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "sequence_domain_{0}.puml";
    private readonly INamingProcessor _namingProcessor;

    public DomainOnlySequenceDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyClassesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public DomainOnlyClassesDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.ClassDomainPumlFilename(contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyStatesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "state_action_{0}.puml";
    private readonly INamingProcessor _namingProcessor;

    public ActionOnlyStatesDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlySequenceDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "sequence_action_{0}.puml";
    private readonly INamingProcessor _namingProcessor;

    public ActionOnlySequenceDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyClassesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public ActionOnlyClassesDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.ClassActionPumlFilename(contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionSummaryDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public ActionSummaryDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => _namingProcessor.ClassActionDomainPumlFilename(contextKey.Action, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ClassOnlyDatamodel<TTensor> : PumlEmbeddedContentDatamodel<TTensor>, IPumlEnbeddedInjectionDatamodel<TTensor>
    where TTensor : notnull
{
    private readonly INamingProcessor _namingProcessor;

    public ClassOnlyDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => _namingProcessor.ClassOnlyPumlFilename(contextKey.AlphanumericOnly());
}

internal class ClassOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly INamingProcessor _namingProcessor;

    public ClassOnlyMindmapDatamodel(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    protected override string GetPumlFileName(TDataTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => _namingProcessor.MindmapClassOnlyPumlFilename(contextKey.AlphanumericOnly());
}

