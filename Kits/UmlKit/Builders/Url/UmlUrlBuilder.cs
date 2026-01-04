using System;
using ContextKit.ContextData;
using ContextKit.ContextData.Naming;
using ContextKit.Model;

namespace UmlKit.Builders.Url;

public class UmlUrlBuilder : IUmlUrlBuilder
{
    private readonly INamingProcessor _namingProcessor;

    public UmlUrlBuilder(INamingProcessor namingProcessor) => _namingProcessor = namingProcessor;

    public string? BuildContextInfoUrl(IContextInfo? contextInfo)
    {
        if (contextInfo == null)
        {
            return null;
        }
        return contextInfo.ElementType.IsEntityDefinition()
            ? _namingProcessor.ClassOnlyHtmlFilename(contextInfo.FullName)
            : null;
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
        var compositeDomainHtmlFile = _namingProcessor.CompositeDomainHtmlFile(domain);
        return string.IsNullOrWhiteSpace(domain)
            ? null
            : $"{compositeDomainHtmlFile}?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }

    public string? BuildActionUrl(string? action)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var compositeActionHtmlFile = _namingProcessor.CompositeActionHtmlFile(action);
        return string.IsNullOrWhiteSpace(action)
            ? null
            : $"{compositeActionHtmlFile}?v={timestamp}&t=MindmapTab";//.AlphanumericOnly()
    }
}