using System;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;

namespace UmlKit.Builders.Url;

public class UmlUrlBuilder : IUmlUrlBuilder
{
    private readonly INamingProcessor _namingProcessor;

    public UmlUrlBuilder(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }

    public string? BuildContextInfoUrl(IContextInfo? contextInfo)
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

    public string? BuildNamespaceUrl(string? nameSpace)
    {
        return string.IsNullOrWhiteSpace(nameSpace)
            ? null
            : _namingProcessor.NamespaceOnlyHtmlFilename(nameSpace);
    }

    public string? BuildDomainUrl(string? domain)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return string.IsNullOrWhiteSpace(domain)
            ? null
            : $"composite_domain_{domain}.html?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }

    public string? BuildActionUrl(string? action)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return string.IsNullOrWhiteSpace(action)
            ? null
            : $"composite_action_{action}.html?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }
}