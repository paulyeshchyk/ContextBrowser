﻿using HtmlKit.Builders.Core;
using HtmlKit.Classes;

namespace HtmlKit.Page;

// pattern: Builder
// pattern note: weak
// context: html, build
public static class HtmlBuilderFactory
{
    // Общие теги, поведение которых не сильно отличается от базового HtmlBuilderBase
    public static readonly IHtmlBuilder Html = new HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);

    public static readonly IHtmlBuilder Head = new HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlBuilder Body = new HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);
    public static readonly IHtmlBuilder Table = new HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);

    // Теги с уникальным поведением
    public static readonly IHtmlBuilder Title = new HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);

    public static readonly IHtmlBuilder Meta = new HtmlBuilderMeta("meta");
    public static readonly IHtmlBuilder Script = new HtmlBuilderScript("script");
    public static readonly IHtmlBuilder Style = new HtmlBuilderStyle("style");
    public static readonly IHtmlBuilder H1 = new HtmlBuilderH1("h1");
    public static readonly IHtmlBuilder Paragraph = new HtmlBuilderP("p");
    public static readonly IHtmlBuilder Ul = new HtmlBuilderUl("ul");
    public static readonly IHtmlBuilder Li = new HtmlBuilderLi("li");
    public static readonly IHtmlCellBuilder Raw = new RawBuilder();

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableCell
    {
        public static readonly IHtmlBuilder SummaryCaption = new CellBuilder(HtmlTagClasses.Cell.SummaryCaption);
        public static readonly IHtmlBuilder TotalSummary = new CellBuilder(HtmlTagClasses.Cell.TotalSummary);
        public static readonly IHtmlBuilder ColSummary = new CellBuilder(HtmlTagClasses.Cell.ColSummary);
        public static readonly IHtmlBuilder ColMeta = new CellBuilder(HtmlTagClasses.Cell.ColMeta);
        public static readonly IHtmlBuilder RowMeta = new CellBuilder(HtmlTagClasses.Cell.RowMeta);
        public static readonly IHtmlBuilder RowSummary = new CellBuilder(HtmlTagClasses.Cell.RowSummary);
        public static readonly IHtmlBuilder Data = new CellBuilder(HtmlTagClasses.Cell.Data);
        public static readonly IHtmlBuilder ActionDomain = new CellBuilder(HtmlTagClasses.Cell.ActionDomain);
    }

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableRow
    {
        public static readonly IHtmlBuilder Summary = new RowBuilder(HtmlTagClasses.Row.Summary);
        public static readonly IHtmlBuilder Data = new RowBuilder(HtmlTagClasses.Row.Data);
        public static readonly IHtmlBuilder Meta = new RowBuilder(HtmlTagClasses.Row.Meta);
    }

    // context: html, build
    internal class HtmlBuilderStandard : HtmlBuilder
    {
        public HtmlBuilderStandard(string tag, string className) : base(tag, className)
        {
        }

        // Cell метод не всегда применим для произвольных тегов (html, head, body, table)
        // Поэтому здесь можно бросить исключение или просто оставить его неиспользуемым,
        // если мы уверены, что для этих тегов Cell вызываться не будет.
        // Для чистоты, можно сделать его более строгим:
        public override void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
        {
            throw new NotSupportedException($"Cell method is not supported for <{Tag}> tag in StandardTagBuilder. Use Start/End instead.");
        }
    }

    // Специализированный билдер для <title>
    // context: html, build
    internal class HtmlBuilderTitle : HtmlBuilder
    {
        public HtmlBuilderTitle(string tag, string className) : base(tag, className)
        {
        }

        public override void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
        {
            WriteContentTag(sb, innerHtml, style);
        }
    }

    // Специализированный билдер для <meta>
    // context: html, build
    internal class HtmlBuilderMeta : HtmlBuilder
    {
        public HtmlBuilderMeta(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <script>
    // context: html, build
    internal class HtmlBuilderScript : HtmlBuilder
    {
        public HtmlBuilderScript(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <style>
    // context: html, build
    internal class HtmlBuilderStyle : HtmlBuilder
    {
        public HtmlBuilderStyle(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <h1>
    private class HtmlBuilderH1 : HtmlBuilder
    {
        public HtmlBuilderH1(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderP : HtmlBuilder
    {
        public HtmlBuilderP(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderUl : HtmlBuilder
    {
        public HtmlBuilderUl(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderLi : HtmlBuilder
    {
        public HtmlBuilderLi(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class CellBuilder : HtmlBuilder
    {
        public CellBuilder(string className) : base("td", className)
        {
        }
    }

    private class RowBuilder : HtmlBuilder
    {
        public RowBuilder(string className) : base("tr", className)
        {
        }
    }

    // context: html, build
    public readonly struct RawBuilder : IHtmlCellBuilder
    {
        // context: html, build
        public void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
        {
            if(!string.IsNullOrWhiteSpace(innerHtml))
                sb.WriteLine(innerHtml);
        }

        // context: html, build
        public void With(TextWriter writer, Action block)
        {
            block?.Invoke();
        }
    }
}