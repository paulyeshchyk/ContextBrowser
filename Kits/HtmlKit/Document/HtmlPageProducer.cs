using System.Collections.Generic;
using System.IO;
using ContextBrowserKit.Options;
using HtmlKit.Builders.Core;
using HtmlKit.Matrix;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

// context: html, model
public abstract class HtmlPageProducer
{
    public string Title { get; set; } = string.Empty;

    protected readonly IAppOptionsStore _optionsStore;

    public HtmlPageProducer(IAppOptionsStore optionsStore)
    {
        _optionsStore = optionsStore;
    }

    protected void Produce(TextWriter writer, IHtmlMatrix matrix)
    {
        var options = _optionsStore.GetOptions<HtmlTableOptions>();

        HtmlBuilderFactory.Html.With(writer, () =>
        {
            HtmlBuilderFactory.Head.With(writer, () =>
            {
                var attrs = new HtmlTagAttributes() { { "charset", "UTF-8" } };
                HtmlBuilderFactory.Meta.Cell(writer, attributes: attrs, isEncodable: false);
                HtmlBuilderFactory.Title.Cell(writer, innerHtml: Title);
                HtmlBuilderFactory.Style.Cell(writer, innerHtml: Resources.HtmlProducerContentStyle, isEncodable: false);

                foreach (var script in GetScripts())
                    HtmlBuilderFactory.Script.Cell(writer, innerHtml: script, isEncodable: false);

                foreach (var script in GetAdditionalScripts())
                    HtmlBuilderFactory.Script.Cell(writer, innerHtml: script, isEncodable: false);

                HtmlBuilderFactory.Body.With(writer, () =>
                {
                    HtmlBuilderFactory.H1.Cell(writer, innerHtml: Title);
                    WriteContent(writer, matrix, options);
                });
            });
        });
    }

    protected abstract IEnumerable<string> GetAdditionalScripts();

    protected IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript;
    }

    protected abstract void WriteContent(TextWriter sb, IHtmlMatrix matrix, HtmlTableOptions options);
}