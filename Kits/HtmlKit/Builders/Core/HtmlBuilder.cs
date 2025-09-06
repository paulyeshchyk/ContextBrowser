using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlKit.Builders.Tag;

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

    public virtual void Start(TextWriter sb, IHtmlTagAttributes? attrs = null)
    {
        var attributes = new HtmlTagAttributes(StringComparer.OrdinalIgnoreCase);
        attributes.Concat(attrs);
        attributes.AddIfNotExists("class", ClassName);

        var result = XMLTagBuilder.BuildStart(Tag, attributes);
        if (!string.IsNullOrWhiteSpace(result))
        {
            sb.WriteLine(result);
        }
    }

    public virtual void End(TextWriter sb)
    {
        var result = XMLTagBuilder.BuildEnd(Tag, IsClosable);

        if (!string.IsNullOrWhiteSpace(result))
        {
            sb.WriteLine(result);
        }
    }

    public virtual void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
    {
        var content = !string.IsNullOrWhiteSpace(innerHtml)
            ? innerHtml
            : string.Empty;
        var attrs = new HtmlTagAttributes(StringComparer.OrdinalIgnoreCase);
        attrs.Concat(attributes);
        attrs.AddIfNotExists("class", ClassName);

        WriteContentTag(sb, attrs, content, isEncodable);
    }

    public virtual HtmlBuilder OnClick(string eventScript)
    {
        _onClickEvent = eventScript;
        return this;
    }

    protected virtual void WriteContentTag(TextWriter sb, IHtmlTagAttributes? attributes, string content = "", bool isEncodable = true)
    {
        var attributesString = attributes?.ToString();
        var contentString = isEncodable
            ? WebUtility.HtmlEncode(content)
            : content;

        string text = XMLTagBuilder.Build(Tag, attributes, IsClosable, contentString);
        sb.WriteLine(text);
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