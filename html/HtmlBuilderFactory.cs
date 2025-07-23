namespace ContextBrowser.html;

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
    public static readonly IHtmlBuilder H1 = new HtmlBuilderH1("h1", HtmlTagClasses.Page.H1);

    private class HtmlBuilderStandard : HtmlBuilder
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
    private class HtmlBuilderTitle : HtmlBuilder
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
    private class HtmlBuilderMeta : HtmlBuilder
    {
        public HtmlBuilderMeta(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <script>
    private class HtmlBuilderScript : HtmlBuilder
    {
        public HtmlBuilderScript(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <style>
    private class HtmlBuilderStyle : HtmlBuilder
    {
        public HtmlBuilderStyle(string tag) : base(tag, string.Empty)
        {
        }
    }

    // Специализированный билдер для <h1>
    private class HtmlBuilderH1 : HtmlBuilder
    {
        public HtmlBuilderH1(string tag, string className) : base(tag, className)
        {
        }
    }
}
