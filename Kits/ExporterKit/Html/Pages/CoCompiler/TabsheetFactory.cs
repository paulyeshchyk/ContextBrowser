using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using HtmlKit.Builders.Core;
using HtmlKit.Document;
using HtmlKit.Matrix;
using HtmlKit.Model;
using HtmlKit.Model.Containers;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using TensorKit.Factories;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Pages.CoCompiler;

public static class TabsheetFactory
{
    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> MindmapTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Mindmap (PUML)
        return TabRegistration.For<MindmapDatamodel, ContextInfoKeyContainerEntityName>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new MindmapDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerEntityName> ClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ClassOnlyDatamodel, ContextInfoKeyContainerEntityName>(
            tabId: "ClassesTab",
            caption: "Общая",
            isActive: true,
            model: new ClassOnlyDatamodel(),
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
        return TabRegistration.For<NamespaceOnlyDatamodel, ContextInfoKeyContainerNamespace>(
            tabId: "NamespacesTab",
            caption: "Общая",
            isActive: true,
            model: new NamespaceOnlyDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> ActionOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionOnlyClassesDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionOnlyClassesDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> MethodsTabRegistration(IHtmlTensorWriter<MethodListTensor> matrixWriter, ITensorFactory<MethodListTensor> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionOnlyMethodListDataModel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionOnlyMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var theAction = (string)dto.ContextKey.Action;
                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{theAction.ToUpper()} -> {dto.ContextKey.Domain}");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> StatesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyStatesDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new ActionOnlyStatesDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> SequenceTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<ActionOnlySequenceDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new ActionOnlySequenceDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor> matrixWriter, ITensorFactory<MethodListTensor> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainSummaryMethodListDataModel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainSummaryMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var theAction = (string)dto.ContextKey.Action;
                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{theAction.ToUpper()} -> {dto.ContextKey.Domain}");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainSummaryComponentsDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainSummaryComponentsDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyMindmap(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyMindmapDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "MindmapTab",
            caption: "Mindmap",
            isActive: false,
            model: new DomainOnlyMindmapDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyStates(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyStatesDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "StatesTab",
            caption: "Состояния",
            isActive: false,
            model: new DomainOnlyStatesDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlySequence(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<DomainOnlySequenceDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "SequenceTab",
            caption: "Последовательности",
            isActive: false,
            model: new DomainOnlySequenceDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyMethodsTabsheetRegistration(IHtmlTensorWriter<MethodListTensor> matrixWriter, ITensorFactory<MethodListTensor> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<DomainOnlyMethodListDataModel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new DomainOnlyMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var theDomain = (string)dto.ContextKey.Domain;
                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{theDomain.ToUpper()} -> {dto.ContextKey.Domain}");

                var methodNames = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods(methodNames, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> DomainOnlyClassesTabsheetRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainOnlyClassesDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new DomainOnlyClassesDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlBuilder = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlBuilder.Start(writer);
                pumlBuilder.Cell(writer);
                pumlBuilder.End(writer);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> ActionPerDomainMethodsTabRegistration(IHtmlTensorWriter<MethodListTensor> matrixWriter, ITensorFactory<MethodListTensor> keyFactory)
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainMethodListDataModel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var theAction = (string)dto.ContextKey.Action;
                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{theAction.ToUpper()} -> {dto.ContextKey.Domain}");

                var methodIndexList = model.GetMethodsList(dto).Select((m, index) => index);
                IHtmlMatrix methodsMatrix = new HtmlMatrixMethods(methodIndexList, dto);
                var options = new HtmlTableOptions(summaryPlacement: SummaryPlacementType.None, orientation: TensorPermutationType.Standard) { };
                matrixWriter.Write(writer, methodsMatrix, null, options);
            });
    }

    public static IHtmlTabRegistration<ContextInfoKeyContainerTensor<DomainPerActionTensor>> ActionPerDomainClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionSummaryDatamodel, ContextInfoKeyContainerTensor<DomainPerActionTensor>>(
            tabId: "ClassesTab",
            caption: "Классы",
            isActive: true,
            model: new ActionSummaryDatamodel(),
            build: (writer, model, dto) =>
            {
                var pumlInjection = model.GetPumlBuilder(dto.ContextKey, exportOptions);
                pumlInjection.Start(writer);
                pumlInjection.Cell(writer);
                pumlInjection.End(writer);
            });
    }
}

internal abstract class PumlEmbeddedContentDatamodelDomainPerAction : PumlEmbeddedContentDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionPerDomainSummaryMethodListDataModel : IMethodListDatamodel<DomainPerActionTensor>
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class ActionPerDomainMethodListDataModel : IMethodListDatamodel<DomainPerActionTensor>
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class DomainOnlyMethodListDataModel : IMethodListDatamodel<DomainPerActionTensor>
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class ActionOnlyMethodListDataModel : IMethodListDatamodel<DomainPerActionTensor>
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextInfoKeyContainerTensor<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class NamespaceOnlyDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"namespace_only_{contextKey.AlphanumericOnly()}.puml";
}

internal class DomainSummaryComponentsDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_{contextKey.Action}_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyStatesDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"state_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyMindmapDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"mindmap_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlySequenceDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"sequence_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyClassesDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyStatesDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"state_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlySequenceDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"sequence_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyClassesDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionSummaryDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_{contextKey.Action}_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ClassOnlyDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"class_only_{contextKey.AlphanumericOnly()}.puml";
}

internal class MindmapDatamodel : PumlEmbeddedContentDatamodelDomainPerAction, IPumlEnbeddedInjectionDatamodel<DomainPerActionTensor>
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"mindmap.puml";
}

