using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Model;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Pages.CoCompiler;

public static class TabsheetFactory
{
    public static IHtmlTabRegistration<EntitynameContainer> MindmapTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Mindmap (PUML)
        return TabRegistration.For<MindmapDatamodel, EntitynameContainer>(
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

    public static IHtmlTabRegistration<EntitynameContainer> ClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ClassOnlyDatamodel, EntitynameContainer>(
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

    public static IHtmlTabRegistration<NamespacenameContainer> NamespaceTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<NamespaceOnlyDatamodel, NamespacenameContainer>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> ActionOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionOnlyClassesDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> MethodsTabRegistration()
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionOnlyMethodListDataModel, ContextKeyContainer<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionOnlyMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var methods = model.GetMethodsList(dto);

                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{dto.ContextKey.Action.ToUpper()}");
                HtmlBuilderFactory.P.Cell(writer, innerHtml: $"Methods: {methods.Count()}");

                HtmlBuilderFactory.Ul.With(writer, () =>
                {
                    foreach (var method in methods.Distinct())
                        HtmlBuilderFactory.Li.Cell(writer, innerHtml: method.FullName);
                });
            });
    }

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> StatesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<ActionOnlyStatesDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> SequenceTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<ActionOnlySequenceDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyMethodsTabRegistration()
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainSummaryMethodListDataModel, ContextKeyContainer<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainSummaryMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var methods = model.GetMethodsList(dto);

                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{dto.ContextKey.Action.ToUpper()} -> {dto.ContextKey.Domain}");
                HtmlBuilderFactory.P.Cell(writer, innerHtml: $"Methods: {methods.Count()}");

                HtmlBuilderFactory.Ul.With(writer, () =>
                {
                    foreach (var method in methods.Distinct())
                        HtmlBuilderFactory.Li.Cell(writer, innerHtml: method.FullName);
                });
            });
    }

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainSummaryComponentsDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyMindmap(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyMindmapDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyStates(ExportOptions exportOptions)
    {
        // Вкладка: Состояния (PUML)
        return TabRegistration.For<DomainOnlyStatesDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlySequence(ExportOptions exportOptions)
    {
        // Вкладка: Компоненты (PUML)
        return TabRegistration.For<DomainOnlySequenceDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyMethodsTabsheetRegistration()
    {
        // Вкладка: Методы
        return TabRegistration.For<DomainOnlyMethodListDataModel, ContextKeyContainer<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new DomainOnlyMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var methods = model.GetMethodsList(dto);

                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{dto.ContextKey.Domain.ToUpper()}");
                HtmlBuilderFactory.P.Cell(writer, innerHtml: $"Methods: {methods.Count()}");

                HtmlBuilderFactory.Ul.With(writer, () =>
                {
                    foreach (var method in methods.Distinct())
                        HtmlBuilderFactory.Li.Cell(writer, innerHtml: method.FullName);
                });
            });
    }

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> DomainOnlyClassesTabsheetRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<DomainOnlyClassesDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> ActionPerDomainMethodsTabRegistration()
    {
        // Вкладка: Методы
        return TabRegistration.For<ActionPerDomainMethodListDataModel, ContextKeyContainer<DomainPerActionTensor>>(
            tabId: "MethodsTab",
            caption: "Методы",
            isActive: false,
            model: new ActionPerDomainMethodListDataModel(),
            build: (writer, model, dto) =>
            {
                var methods = model.GetMethodsList(dto);

                HtmlBuilderFactory.H1.Cell(writer, innerHtml: $"{dto.ContextKey.Action.ToUpper()} -> {dto.ContextKey.Domain}");
                HtmlBuilderFactory.P.Cell(writer, innerHtml: $"Methods: {methods.Count()}");

                HtmlBuilderFactory.Ul.With(writer, () =>
                {
                    foreach (var method in methods.Distinct())
                        HtmlBuilderFactory.Li.Cell(writer, innerHtml: method.FullName);
                });
            });
    }

    public static IHtmlTabRegistration<ContextKeyContainer<DomainPerActionTensor>> ActionPerDomainClassesTabRegistration(ExportOptions exportOptions)
    {
        // Вкладка: Классы (PUML)
        return TabRegistration.For<ActionSummaryDatamodel, ContextKeyContainer<DomainPerActionTensor>>(
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

internal class ActionPerDomainSummaryMethodListDataModel : IMethodListDatamodel
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class ActionPerDomainMethodListDataModel : IMethodListDatamodel
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class DomainOnlyMethodListDataModel : IMethodListDatamodel
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class ActionOnlyMethodListDataModel : IMethodListDatamodel
{
    public IEnumerable<IContextInfo> GetMethodsList(ContextKeyContainer<DomainPerActionTensor> dto) => dto.ContextInfoList;
}

internal class NamespaceOnlyDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"namespace_only_{contextKey.AlphanumericOnly()}.puml";
}

internal class DomainSummaryComponentsDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_{contextKey.Action}_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyStatesDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"state_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyMindmapDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"mindmap_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlySequenceDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"sequence_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class DomainOnlyClassesDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_domain_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyStatesDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"state_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlySequenceDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"sequence_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionOnlyClassesDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_action_{contextKey.Action}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ActionSummaryDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => $"class_{contextKey.Action}_{contextKey.Domain}.puml";

    protected override string GetPumlFileName(string contextKey) => throw new NotImplementedException();
}

internal class ClassOnlyDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"class_only_{contextKey.AlphanumericOnly()}.puml";
}

internal class MindmapDatamodel : PumlEmbeddedContentDatamodel, IPumlEnbeddedInjectionDatamodel
{
    protected override string GetPumlFileName(DomainPerActionTensor contextKey) => throw new NotImplementedException();

    protected override string GetPumlFileName(string contextKey) => $"mindmap.puml";
}

