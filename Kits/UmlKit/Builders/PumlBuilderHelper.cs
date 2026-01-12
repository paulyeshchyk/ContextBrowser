using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Extensions;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.Builders;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders;

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
            var visibility = element.ContextInfo.ElementVisibility.ToUmlMemberVisibility();
            var umlMethod = new UmlMethod(element.ContextInfo.Name + "()".PadRight(maxLength), visibility: visibility, url: null);
            umlClass.Add(umlMethod);
        }

        return umlClass;
    }

    public static UmlEntity BuildUmlEntity(IContextInfo classownerInfo)
    {
        var entityType = classownerInfo.ElementType.ConvertToUmlEntityType();
        return new UmlEntity(entityType, classownerInfo.Name, classownerInfo.Name.AlphanumericOnly(), url: null);
    }

    public static UmlComponent BuildUmlComponent(int maxLength, ContextInfo method)
    {
        return new UmlComponent(method.Name.PadRight(maxLength));
    }

    public static UmlEntity BuildUmlEntityClass(ContextInfo contextInfo, IEnumerable<ContextInfo> methsonly, IEnumerable<ContextInfo> propsonly, int maxLength, INamingProcessor namingProcessor)
    {
        var classNameWithNameSpace = $"{contextInfo.Namespace}.{contextInfo.ShortName}";
        var htmlUrl = namingProcessor.ClassOnlyHtmlFilename(classNameWithNameSpace);

        var entityType = contextInfo.ElementType.ConvertToUmlEntityType();
        var umlClass = new UmlEntity(entityType, contextInfo.Name.PadRight(maxLength), contextInfo.FullName.AlphanumericOnly(), url: htmlUrl);

        PumlBuilderHelper.AddUmlProperties(umlClass, propsonly, maxLength);

        PumlBuilderHelper.AddUmlMethods(umlClass, methsonly, maxLength);
        return umlClass;
    }

    public static void AddMethodBox(UmlComponentGroup compGroup, ContextInfo method)
    {
        var methodStereotype = string.Join(", ", method.Contexts.Distinct().OrderBy(x => x));
        var methodBox = new UmlMethodBox(CleanName(method.Name), methodStereotype);
        compGroup.Add(methodBox);
    }

    public static void AddComponentGroup(List<ContextInfo> methods, UmlPackage package, ContextInfo cls)
    {
        var stereotype = cls.Contexts.Count != 0
            ? string.Join(", ", cls.Contexts.OrderBy(c => c))
            : "NoContext";

        var compGroup = new UmlComponentGroup(CleanName(cls.Name), stereotype);

        var methodsInClass = methods.Where(m => m.ClassOwner?.FullName == cls.FullName);
        foreach (var method in methodsInClass)
        {
            PumlBuilderHelper.AddMethodBox(compGroup, method);
        }

        package.Add(compGroup);
    }

    // context: uml, build
    private static string? CleanName(string? rawName)
    {
        if (string.IsNullOrEmpty(rawName))
            return rawName;

        return Regex.Replace(rawName, @"<(.+?)>", "[$1]")
                    .Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;");
    }

    public static UmlPackage BuildUmlPackage(string nameSpace, IUmlUrlBuilder umlUrlBuilder, int maxLength = 0)
    {
        var packageUrl = umlUrlBuilder.BuildNamespaceUrl(nameSpace);
        var package = new UmlPackage(nameSpace.PadRight(maxLength), alias: nameSpace.AlphanumericOnly(), url: packageUrl);
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

        var umlClass = PumlBuilderHelper.BuildUmlEntity(classownerInfo.Name, htmlUrl, entityType, maxLength);

        var classMethods = onGetMethods(classownerInfo);
        PumlBuilderHelper.AddUmlMethods(umlClass, classMethods);

        var classProperties = onGetProperties(classownerInfo);
        PumlBuilderHelper.AddUmlProperties(umlClass, classProperties,0);

        return umlClass;
    }

    public static UmlEntity BuildUmlEntity(string entityName, string htmlUrl, UmlEntityType entityType, int maxLength)
    {
        return new UmlEntity(entityType, entityName.PadRight(maxLength), entityName.AlphanumericOnly(), url: htmlUrl);
    }

    public static UmlTransitionState<UmlState> BuildUmlTransitionState(ContextInfo method, ContextInfo callee)
    {
        var state1 = new UmlState(string.IsNullOrWhiteSpace(method.Name) ? "<unknown method>" : method.Name, null);
        var state2 = new UmlState(string.IsNullOrWhiteSpace(callee.Name) ? "<unknown callee>" : callee.Name, null);
        var arrow = new UmlArrow();
        return new UmlTransitionState<UmlState>(state1, state2, arrow);
    }

    public static UmlRelation BuildUmlRelation(UmlDiagramClass diagram, string from, string to, DiagramBuilderOptions options)
    {
        var arrow = new UmlArrow(flowType: options.Indication.UseAsync ? UmlArrowFlowType.Async : UmlArrowFlowType.Sync);
        return new UmlRelation(from, to, arrow);
    }

    public static void AddUmlMethods(UmlEntity umlClass, IEnumerable<IContextInfo>? methodList, int maxLength = 0)
    {
        if (methodList == null || !methodList.Any())
            return;

        foreach (var element in methodList)
        {
            var visibility = element.ElementVisibility.ToUmlMemberVisibility();
            var urlMethod = new UmlMethod(element.ShortName + SParentheses.PadRight(maxLength), visibility: visibility, url: null);
            umlClass.Add(urlMethod);
        }
    }

    public static void AddUmlProperties(UmlEntity umlClass, IEnumerable<IContextInfo>? propertyList, int maxLength = 0)
    {
        if (propertyList == null || !propertyList.Any())
            return;

        foreach (var element in propertyList)
        {
            var visibility = element.ElementVisibility.ToUmlMemberVisibility();
            var umlProperty = new UmlProperty(element.ShortName.PadRight(maxLength), visibility: visibility, url: null);
            umlClass.Add(umlProperty);
        }
    }
}

internal static class VisiblityConverter
{
    public static UmlMemberVisibility ToUmlMemberVisibility(this ContentInfoElementVisibility elementType)
    {
        return elementType switch
        {
            ContentInfoElementVisibility.@private => UmlMemberVisibility.@private,
            ContentInfoElementVisibility.@protected => UmlMemberVisibility.@protected,
            ContentInfoElementVisibility.@public => UmlMemberVisibility.@public,
            ContentInfoElementVisibility.@internal => UmlMemberVisibility.@internal,
            _ => throw new NotImplementedException(),
        };
    }
}