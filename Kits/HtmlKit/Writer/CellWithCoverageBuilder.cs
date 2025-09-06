using System.Collections.Generic;
using System.IO;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Page;

namespace HtmlKit.Writer;

// pattern: Builder
internal class CellWithCoverageBuilder
{
    public static string Build(IContextKey container, int cnt)
    {
        if (cnt == 0)
        {
            return string.Empty;
        }

        using var sw = new StringWriter();

        var attributes = new HtmlTagAttributes() { { "class", "embedded-table" } };
        Page.HtmlBuilderFactory.Table.With(sw, attributes, () =>
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