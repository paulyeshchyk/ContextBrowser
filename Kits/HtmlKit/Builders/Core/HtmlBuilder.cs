using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Builders.Core;

//pattern: Builder, template method
public abstract class HtmlBuilder : IHtmlBuilder
{
    protected readonly string Tag;
    protected readonly string ClassName;
    protected string _onClickEvent = string.Empty;

    protected virtual bool isRaw => false;

    protected virtual bool IsClosable => true;

    protected HtmlBuilder(string tag, string className)
    {
        Tag = tag;
        ClassName = className;
    }

    public virtual Task StartAsync(TextWriter sb, IHtmlTagAttributes? attrs = null, CancellationToken cancellationToken = default)
    {
        var attributes = new HtmlTagAttributes(StringComparer.OrdinalIgnoreCase);
        attributes.Concat(attrs);
        attributes.AddIfNotExists("class", ClassName);

        var result = XMLTagBuilder.BuildStart(Tag, attributes);
        if (!string.IsNullOrWhiteSpace(result))
        {
            sb.WriteLine(result);
        }
        return Task.CompletedTask;
    }

    public virtual Task EndAsync(TextWriter sb, CancellationToken cancellationToken = default)
    {
        var result = XMLTagBuilder.BuildEnd(Tag, IsClosable);

        if (!string.IsNullOrWhiteSpace(result))
        {
            sb.WriteLine(result);
        }
        return Task.CompletedTask;
    }

    public virtual async Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
    {
        var content = !string.IsNullOrWhiteSpace(innerHtml)
            ? innerHtml
            : string.Empty;
        var attrs = new HtmlTagAttributes(StringComparer.OrdinalIgnoreCase);
        attrs.Concat(attributes);
        attrs.AddIfNotExists("class", ClassName);

        await WriteContentTagAsync(sb, attrs, content, isEncodable, cancellationToken).ConfigureAwait(false);
    }

    public virtual HtmlBuilder OnClick(string eventScript)
    {
        _onClickEvent = eventScript;
        return this;
    }

    protected virtual Task WriteContentTagAsync(TextWriter sb, IHtmlTagAttributes? attributes, string content = "", bool isEncodable = true, CancellationToken cancellationToken = default)
    {
        var attributesString = attributes?.ToString();
        var contentString = isEncodable
            ? WebUtility.HtmlEncode(content)
            : content;

        string text = XMLTagBuilder.Build(Tag, attributes, IsClosable, contentString);
        sb.WriteLine(text);
        return Task.CompletedTask;
    }
}

public static class XMLTagBuilder
{
    public static string Build(string Tag, IHtmlTagAttributes? attributes, bool IsClosable, string contentString)
    {
        var list = new SortedList<int, string?>();

        list.Add(1, XMLTagBuilder.BuildStart(Tag, attributes));

        if (!string.IsNullOrWhiteSpace(contentString))
            list.Add(2, contentString);

        list.Add(3, XMLTagBuilder.BuildEnd(Tag, IsClosable));

        var text = string.Concat(list.OrderBy(p => p.Key).Select(p => p.Value).Cast<string>());
        return text;
    }

    public static string BuildStart(string Tag, IHtmlTagAttributes? attributes)
    {
        var attributesStr = attributes?.ToString();

        var list = new List<string>();
        list.Add($"<{Tag}");
        if (!string.IsNullOrWhiteSpace(attributesStr))
        {
            list.Add($" {attributesStr}");
        }
        list.Add(">");

        return string.Concat(list);
    }

    public static string? BuildEnd(string Tag, bool IsClosable)
    {
        return (IsClosable)
            ? $"</{Tag}>"
            : null;
    }
}