using System.Collections.Generic;
using System.IO;
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
                var attrs = new HtmlTagAttributes() { { "charset", "UTF-8" } };
                HtmlBuilderFactory.Meta.Cell(writer, attributes: attrs, isEncodable: false);
                HtmlBuilderFactory.Title.Cell(writer, innerHtml:Title);
                HtmlBuilderFactory.Style.Cell(writer, innerHtml:Resources.HtmlProducerContentStyle, isEncodable: false);

                foreach (var script in GetScripts())
                    HtmlBuilderFactory.Script.Cell(writer,innerHtml:script, isEncodable: false);

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, innerHtml:Title);
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