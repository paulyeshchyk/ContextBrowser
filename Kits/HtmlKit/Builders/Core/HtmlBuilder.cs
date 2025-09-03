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

        var attributesStr = attributes.ToString();
        sb.WriteLine($"<{Tag} {attributesStr}>");
    }

    public virtual void End(TextWriter sb)
    {
        if (IsClosable)
        {
            sb.WriteLine($"</{Tag}>");
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
        var str = attributes?.ToString();
        var theContent = isEncodable
            ? WebUtility.HtmlEncode(content)
            : content;

        sb.WriteLine($"<{Tag}{(string.IsNullOrWhiteSpace(str) ? "" : " ")}{str}>{theContent}</{Tag}>");
    }
}