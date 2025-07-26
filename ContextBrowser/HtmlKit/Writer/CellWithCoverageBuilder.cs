using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Page;

namespace ContextBrowser.HtmlKit.Writer;

// pattern: Builder
internal class CellWithCoverageBuilder
{
    public static string Build(ContextContainer container, int cnt)
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
