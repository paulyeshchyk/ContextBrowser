using System.IO;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Document;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// pattern: Builder
public class HtmlContentInjector<TTensor> : IHtmlContentInjector<TTensor>
    where TTensor : notnull
{
    public string Inject(TTensor container, int cnt)
    {
        if (cnt == 0)
        {
            return string.Empty;
        }

        using var sw = new StringWriter();

        var attributes = new HtmlTagAttributes() { { "class", "embedded-table" } };
        HtmlBuilderFactory.Table.With(sw, attributes, () =>
        {
            WriteRow(sw, cnt.ToString());
        });

        return sw.ToString();
    }

    private static void WriteRow(TextWriter writer, string label)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(writer, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.Data.Cell(writer, innerHtml: label, isEncodable: false);
        });
    }
}