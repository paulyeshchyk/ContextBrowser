using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Document;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// pattern: Builder
public class HtmlContentInjector<TTensor> : IHtmlContentInjector<TTensor>
    where TTensor : notnull
{
    public async Task<string> InjectAsync(TTensor container, int cnt, CancellationToken cancellationToken)
    {
        if (cnt == 0)
        {
            return string.Empty;
        }

        using var sw = new StringWriter();

        var attributes = new HtmlTagAttributes() { { "class", "embedded-table" } };
        await HtmlBuilderFactory.Table.WithAsync(sw, attributes, async (token) =>
        {
            await WriteRowAsync(sw, cnt.ToString(), token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);

        return sw.ToString();
    }

    private static async Task WriteRowAsync(TextWriter writer, string label, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableRow.Data.WithAsync(writer, (token) =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.Data.CellAsync(writer, innerHtml: label, isEncodable: false);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }
}