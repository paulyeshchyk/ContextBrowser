using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using ExporterKit.Uml.Model;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Compiler;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace ExporterKit.Uml.Builder;

public static class PumlBuilderHelper
{
    private const string SParentheses = "()";
    public static UmlEntity BuildUmlEntityClass(IContextInfo contextInfo, IGrouping<string, UmlClassDiagramElementDto> classGroup, int maxLength, INamingProcessor _namingProcessor)
    {
        var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";

        var htmlUrl = _namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace);
        var umlClass = new UmlEntity(UmlEntityType.@class, classGroup.Key.PadRight(maxLength), classGroup.Key.AlphanumericOnly(), url: htmlUrl);

        foreach (var element in classGroup)
        {
            string? url = null;//UmlUrlBuilder.BuildUrl(element);
            var umlMethod = new UmlMethod(element.ContextInfo.Name + "()".PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url);
            umlClass.Add(umlMethod);
        }

        return umlClass;
    }

    public static UmlEntity BuildUmlEntity(IContextInfo classownerInfo)
    {
        var entityType = classownerInfo.ElementType.ConvertToUmlEntityType();
        var umlClass = new UmlEntity(entityType, classownerInfo.Name, classownerInfo.Name.AlphanumericOnly(), url: null);
        return umlClass;
    }

    public static UmlComponent BuildUmlComponent(int maxLength, ContextInfo method)
    {
        return new UmlComponent(method.Name.PadRight(maxLength));
    }

    public static UmlEntity BuildUmlEntityClass(ContextInfo contextInfo, List<ContextInfo> methsonly, List<ContextInfo> propsonly, int maxLength, INamingProcessor namingProcessor)
    {
        var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";
        var htmlUrl = namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace);

        var entityType = contextInfo.ElementType.ConvertToUmlEntityType();
        var umlClass = new UmlEntity(entityType, contextInfo.Name.PadRight(maxLength), contextInfo.FullName.AlphanumericOnly(), url: htmlUrl);
        PumlBuilderHelper.BuildUmlClassProps(umlClass, contextInfo, propsonly, maxLength);

        PumlBuilderHelper.BuildUmlClassMeths(umlClass, contextInfo, methsonly, maxLength);
        return umlClass;
    }

    public static UmlPackage BuildUmlPackage(string nameSpace, IUmlUrlBuilder umlUrlBuilder, int maxLength)
    {
        var packageUrl = umlUrlBuilder.BuildNamespaceUrl(nameSpace);
        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: packageUrl);
        return package;
    }

    public static UmlPackage BuildUmlPackage(IContextInfo classownerInfo, IUmlUrlBuilder _umlUrlBuilder)
    {
        var packageUrl = _umlUrlBuilder.BuildNamespaceUrl(classownerInfo.Namespace);
        var package = new UmlPackage(classownerInfo.Namespace, alias: classownerInfo.Namespace.AlphanumericOnly(), url: packageUrl);
        return package;
    }

    public static UmlPackage BuildUmlPackage(IUmlUrlBuilder umlUrlBuilder, string ns)
    {
        var namespaceUrl = umlUrlBuilder.BuildNamespaceUrl(ns);
        var umlPackage = new UmlPackage(ns, alias: ns.AlphanumericOnly(), url: namespaceUrl);
        return umlPackage;
    }

    public static UmlPackage BuildUmlPackage(int maxLength, IGrouping<string, UmlClassDiagramElementDto> nsGroup, IUmlUrlBuilder umlUrlBuilder)
    {
        var namespaceUrl = umlUrlBuilder.BuildNamespaceUrl(nsGroup.Key);
        var package = new UmlPackage(nsGroup.Key.PadRight(maxLength), alias: nsGroup.Key.AlphanumericOnly(), url: namespaceUrl);
        return package;
    }

    public static UmlEntity BuildUmlEntityClass(IContextInfo contextInfo, Func<IContextInfo, IEnumerable<IContextInfo>> onGetMethods, Func<IContextInfo, IEnumerable<IContextInfo>> onGetProperties, int maxLength, INamingProcessor namingProcessor)
    {
        var classownerInfo = contextInfo.MethodOwnedByItSelf
            ? (contextInfo.ClassOwner ?? contextInfo)
            : contextInfo;
        var classNameWithNameSpace = $"{classownerInfo.Namespace}.{classownerInfo.ShortName}";

        var htmlUrl = namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace);
        var entityType = classownerInfo.ElementType.ConvertToUmlEntityType();

        var umlClass = PumlBuilderHelper.BuildUmlEntity(maxLength, classownerInfo, htmlUrl, entityType);
        PumlBuilderHelper.BuildUmlMethods(umlClass, classownerInfo, onGetMethods);
        PumlBuilderHelper.BuildUmlProperties(umlClass, classownerInfo, onGetProperties);
        return umlClass;
    }

    public static UmlEntity BuildUmlEntity(int maxLength, IContextInfo classownerInfo, string htmlUrl, UmlEntityType entityType)
    {
        return new UmlEntity(entityType, classownerInfo.Name.PadRight(maxLength), classownerInfo.Name.AlphanumericOnly(), url: htmlUrl);
    }


    public static UmlTransitionState BuildUmlTransitionState(ContextInfo method, ContextInfo callee)
    {
        var state1 = new UmlState(string.IsNullOrWhiteSpace(method.Name) ? "<unknown method>" : method.Name, null);
        var state2 = new UmlState(string.IsNullOrWhiteSpace(callee.Name) ? "<unknown callee>" : callee.Name, null);
        var arrow = new UmlArrow();
        return new UmlTransitionState(state1, state2, arrow);
    }

    public static void BuildUmlMethods(UmlEntity umlClass, IContextInfo classownerInfo, Func<IContextInfo, IEnumerable<IContextInfo>> onGetMethods)
    {
        var classMethods = onGetMethods(classownerInfo);
        foreach (var element in classMethods)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }
    }

    public static void BuildUmlMethods(UmlEntity umlClass, IContextInfo classownerInfo, Func<IContextInfo, IEnumerable<IContextInfo>> onGetMethods, Func<IContextInfo, IEnumerable<IContextInfo>> onGetProperties)
    {
        var classMethods = onGetMethods(classownerInfo);
        foreach (var element in classMethods)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }

        var classProperties = onGetProperties(classownerInfo);
        foreach (var element in classProperties)
        {
            var umlMethod = new UmlMethod(element.Name + "()", visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }
    }

    public static void BuildUmlClassMeths(UmlEntity umlClass, ContextInfo contextInfo, List<ContextInfo> methodsonly, int maxLength)
    {
        var methodList = methodsonly.Where(m => m.ClassOwner?.FullName.Equals(contextInfo.FullName) ?? false).Distinct();
        foreach (var element in methodList)
        {
            string? url = null;//UmlUrlBuilder.BuildUrl(element);
            umlClass.Add(new UmlMethod(element.ShortName + SParentheses.PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url));
        }
    }

    public static void BuildUmlProperties(UmlEntity umlClass, IContextInfo classownerInfo, Func<IContextInfo, IEnumerable<IContextInfo>> onGetProperties)
    {
        var classProperties = onGetProperties(classownerInfo);
        foreach (var element in classProperties)
        {
            var umlMethod = new UmlMethod(element.Name, visibility: UmlMemberVisibility.@public, url: null);
            umlClass.Add(umlMethod);
        }
    }

    public static void BuildUmlClassProps(UmlEntity umlClass, ContextInfo contextInfo, List<ContextInfo> propsonly, int maxLength)
    {
        var propsList = propsonly.Where(p => p.ClassOwner?.FullName.Equals(contextInfo.FullName) ?? false).Distinct();
        foreach (var element in propsList)
        {
            string? url = null;// UmlUrlBuilder.BuildUrl(element);
            umlClass.Add(new UmlProperty(element.ShortName.PadRight(maxLength), visibility: UmlMemberVisibility.@public, url: url));
        }
    }

}

