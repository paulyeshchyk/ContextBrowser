using System.IO;
using ContextKit.Model.Matrix;
using HtmlKit.Builders.Core;
using HtmlKit.Page;

namespace HtmlKit.Writer;

// pattern: Builder
internal class CellWithCoverageBuilder
{
    public static string Build(ContextInfoDataCell container, int cnt)
    {
        if (cnt == 0)
        {
            return string.Empty;
        }

        using var sw = new StringWriter();

        Page.HtmlBuilderFactory.Table.With(sw, () =>
        {
            WriteRow(sw, cnt.ToString());
        }, className: "embedded-table");

        return sw.ToString();
    }

    private static void WriteRow(TextWriter writer, string label)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(writer, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.Data.Cell(writer, label);
        });
    }
}