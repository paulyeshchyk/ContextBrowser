using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Matrix;

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

    protected async Task ProduceAsync(TextWriter writer, IHtmlMatrix matrix, CancellationToken cancellationToken)
    {
        var options = _optionsStore.GetOptions<HtmlTableOptions>();

        await HtmlBuilderFactory.Html.WithAsync(writer, async (ctoken) =>
        {
            await HtmlBuilderFactory.Head.WithAsync(writer, async (btoken) =>
            {
                var attrs = new HtmlTagAttributes() { { "charset", "UTF-8" } };
                await HtmlBuilderFactory.Meta.CellAsync(writer, attributes: attrs, isEncodable: false, cancellationToken: btoken).ConfigureAwait(false);
                await HtmlBuilderFactory.Title.CellAsync(writer, innerHtml: Title, cancellationToken: btoken).ConfigureAwait(false);
                await HtmlBuilderFactory.Style.CellAsync(writer, innerHtml: Resources.HtmlProducerContentStyle, isEncodable: false, cancellationToken: btoken).ConfigureAwait(false);

                foreach (var script in GetScripts())
                    await HtmlBuilderFactory.Script.CellAsync(writer, innerHtml: script, isEncodable: false, cancellationToken: btoken).ConfigureAwait(false);

                foreach (var script in GetAdditionalScripts())
                    await HtmlBuilderFactory.Script.CellAsync(writer, innerHtml: script, isEncodable: false, cancellationToken: btoken).ConfigureAwait(false);

                await HtmlBuilderFactory.Body.WithAsync(writer, async (atoken) =>
                {
                    await HtmlBuilderFactory.H1.CellAsync(writer, innerHtml: Title, cancellationToken: atoken).ConfigureAwait(false);
                    await WriteContentAsync(writer, matrix, options, cancellationToken: atoken).ConfigureAwait(false);
                }, btoken).ConfigureAwait(false);
            }, ctoken).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    protected abstract IEnumerable<string> GetAdditionalScripts();

    protected IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript;
    }

    protected abstract Task WriteContentAsync(TextWriter sb, IHtmlMatrix matrix, HtmlTableOptions options, CancellationToken cancellationToken);
}