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

    protected void Produce(TextWriter writer)
    {
        HtmlBuilderFactory.Html.With(writer, () =>
        {
            HtmlBuilderFactory.Head.With(writer, () =>
            {
                HtmlBuilderFactory.Meta.Cell(writer, style: "charset =\"UTF-8\"");
                HtmlBuilderFactory.Title.Cell(writer, Title);
                HtmlBuilderFactory.Style.Cell(writer, Resources.HtmlProducerContentStyle);

                foreach (var script in GetScripts())
                    HtmlBuilderFactory.Script.Cell(writer, script);

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, Title);
                    WriteContent(writer);
                });
            });
        });
    }

    protected virtual IEnumerable<string> GetScripts()
    {
        return new List<string> {
            Resources.HtmlProducerContentStyleScript
        };
    }

    protected abstract void WriteContent(TextWriter sb);

    public string ToHtmlString()
    {
        using var sw = new StringWriter();
        Produce(sw);
        return sw.ToString();
    }
}