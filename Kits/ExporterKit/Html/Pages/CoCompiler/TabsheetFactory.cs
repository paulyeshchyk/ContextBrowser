using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
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

    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> DomainMindmapTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Mindmap (PUML)
        return TabRegistration.For<ClassOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerEntityName>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new ClassOnlyMindmapDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> DomainClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ClassOnlyDatamodel<TDataTensor>, ContextInfoKeyContainerEntityName>(
            tabId: "ClassesTab",
            caption: "Общая",
            isActive: true,
            model: new ClassOnlyDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerNamespace> NamespaceTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<NamespaceOnlyDatamodel<TDataTensor>, ContextInfoKeyContainerNamespace>(
            tabId: "NamespacesTab",
            caption: "Общая",
            isActive: true,
            model: new NamespaceOnlyDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionOnlyClassesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionOnlyClassesDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> MethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionOnlyMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionOnlyMethodListDataModel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var titleAction = (string)dto.ContextKey.Action;
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: $"Action: <b>{titleAction}</b>", isEncodable: false);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: "-");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> StatesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyStatesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new ActionOnlyStatesDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> SequenceTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<ActionOnlySequenceDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new ActionOnlySequenceDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<SummaryMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new SummaryMethodListDataModel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: $"Domain: <b>{titleDomain}</b>", isEncodable: false);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: "-");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainSummaryComponentsDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainSummaryComponentsDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionOnlyMindmap(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new ActionOnlyMindmapDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMindmap(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyMindmapDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new DomainOnlyMindmapDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyStates(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyStatesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new DomainOnlyStatesDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlySequence(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<DomainOnlySequenceDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new DomainOnlySequenceDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyMethodsTabsheetRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<DomainOnlyMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new DomainOnlyMethodListDataModel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: $"Domain: <b>{titleDomain}</b>", isEncodable: false);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: "-");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> DomainOnlyClassesTabsheetRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainOnlyClassesDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainOnlyClassesDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionPerDomainMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor<TDataTensor>> matrixWriter, ITensorFactory<MethodListTensor<TDataTensor>> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainMethodListDataModel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainMethodListDataModel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var titleDomain = (string)dto.ContextKey.Domain;
                var linkDomain = string.Format(HtmlPathTemplateCompositeDomain, titleDomain);
                var titleAction = (string)dto.ContextKey.Action;
                var linkAction = string.Format(HtmlPathTemplateCompositeAction, titleAction);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: $"Domain: <a href=\"{linkDomain}\">{titleDomain}</a>", isEncodable: false);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: $"Action: <a href=\"{linkAction}\">{titleAction}</a>", isEncodable: false);
                HtmlBuilderFactory.Div.Cell(writer, innerHtml: "-");

                var methodIndexList = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods<TDataTensor>(methodIndexList, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<TDataTensor>> ActionPerDomainClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionSummaryDatamodel<TDataTensor>, ContextInfoKeyContainerTensor<TDataTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionSummaryDatamodel<TDataTensor>(),
            build: (writer, model, dto) =>
            {
                var pumlInjection = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlInjection.Start(writer);
                pumlInjection.Cell(writer);
                pumlInjection.End(writer);
            });
    }
}

internal class SummaryMethodListDataModel<TDataTensor> : IMethodListDatamodel<TDataTensor>
    where TDataTensor : notnull
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TDataTensor> dto) => dto.ContextInfoList;
}

internal class ActionPerDomainMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class DomainOnlyMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class ActionOnlyMethodListDataModel<TTensor> : IMethodListDatamodel<TTensor>
    where TTensor : notnull
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<TTensor> dto) => dto.ContextInfoList;
}

internal class NamespaceOnlyDatamodel<TTensor> : PumlEmbeddedContentDatamodel<TTensor>, IPumlEnbeddedInjectionDatamodel<TTensor>
    where TTensor : notnull
{
    private const string PumlFilenameTemplate = "namespace_only_{0}.puml";

    protected override string GetPumlFileName(TTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => string.Format(PumlFilenameTemplate, contextKey.AlphanumericOnly());
}

internal class DomainSummaryComponentsDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "class_{0}_{1}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyStatesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "state_domain_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "mindmap_action_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "mindmap_domain_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlySequenceDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "sequence_domain_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyClassesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "class_domain_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyStatesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "state_action_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlySequenceDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "sequence_action_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyClassesDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "class_action_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionSummaryDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "class_{0}_{1}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => string.Format(PumlFilenameTemplate, contextKey.Action, contextKey.Domain);

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ClassOnlyDatamodel<TTensor> : PumlEmbeddedContentDatamodel<TTensor>, IPumlEnbeddedInjectionDatamodel<TTensor>
    where TTensor : notnull
{
    private const string PumlFilenameTemplate = "class_only_{0}.puml";

    protected override string GetPumlFileName(TTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => string.Format(PumlFilenameTemplate, contextKey.AlphanumericOnly());
}

internal class ClassOnlyMindmapDatamodel<TDataTensor> : PumlEmbeddedContentDatamodel<TDataTensor>, IPumlEnbeddedInjectionDatamodel<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    private const string PumlFilenameTemplate = "mindmap_class_only_{0}.puml";

    protected override string GetPumlFileName(TDataTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => string.Format(PumlFilenameTemplate, contextKey.AlphanumericOnly());
}

