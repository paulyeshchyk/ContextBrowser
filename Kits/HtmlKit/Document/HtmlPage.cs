using HtmlKit.Builders.Core;
using HtmlKit.Page;

namespace HtmlKit.Document;

// context: html, model
public abstract class HtmlPage
{
    public string Title { get; set; } = string.Empty;

    public HtmlPage()
    {
    }

    protected void Produce(TextWriter sb)
    {
        HtmlBuilderFactory.Html.With(sb, () =>
        {
            HtmlBuilderFactory.Head.With(sb, () =>
            {
                HtmlBuilderFactory.Meta.Cell(sb, plainText: false, style: "charset =\"UTF-8\"");
                HtmlBuilderFactory.Title.Cell(sb, plainText: true, Title);
                HtmlBuilderFactory.Style.Cell(sb, plainText: false, Resources.HtmlProducerContentStyle);

                foreach (var script in GetScripts())
                    HtmlBuilderFactory.Script.Cell(sb, plainText: false, script);

                HtmlBuilderFactory.Body.With(sb, () =>
                {
                    HtmlBuilderFactory.H1.Cell(sb, plainText: true, Title);
                    WriteContent(sb);
                });
            });
        });
    }

    protected virtual IEnumerable<string> GetScripts()
    {
        // По умолчанию только один скрипт
        yield return Resources.HtmlProducerContentStyleScript;
    }

    protected abstract void WriteContent(TextWriter sb);

    public string ToHtmlString()
    {
        using var sw = new StringWriter();
        Produce(sw);
        return sw.ToString();
    }
}