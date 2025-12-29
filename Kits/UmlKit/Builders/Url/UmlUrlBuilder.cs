using System;
using ContextBrowserKit.Extensions;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders.Url;

public static class UmlUrlBuilder
{
    public static string? BuildContextInfoUrl(IContextInfo? contextInfo, INamingProcessor _namingProcessor)
    {
        if (contextInfo == null)
        {
            return null;
        }

        if ((contextInfo.ElementType == ContextInfoElementType.@class)
            || (contextInfo.ElementType == ContextInfoElementType.record)
            || (contextInfo.ElementType == ContextInfoElementType.@struct))
        {
            return _namingProcessor.ClassOnlyHtmlFilename(contextInfo.FullName);
        }
        else
        {
            return null;
        }
    }

    public static string? BuildNamespaceUrl(string? nameSpace)
    {
        return string.IsNullOrWhiteSpace(nameSpace)
            ? null
            : $"namespace_only_{nameSpace.AlphanumericOnly()}.html";
    }

    public static string? BuildDomainUrl(string? domain)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return string.IsNullOrWhiteSpace(domain)
            ? null
            : $"composite_domain_{domain}.html?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }

    public static string? BuildActionUrl(string? action)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return string.IsNullOrWhiteSpace(action)
            ? null
            : $"composite_action_{action}.html?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }
}