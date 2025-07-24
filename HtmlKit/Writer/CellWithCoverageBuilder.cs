using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Builders.Rows;
using ContextBrowser.HtmlKit.Builders.Tag;

namespace ContextBrowser.HtmlKit.Writer;

// pattern: Builder
internal class CellWithCoverageBuilder
{
    public static string Build(ContextContainer container, int cnt)
    {
        if(cnt == 0)
        {
            return string.Empty;
        }

        using var sw = new StringWriter();

        Page.HtmlBuilderFactory.Table.With(sw,() =>
        {
            WriteRow(sw, cnt.ToString());
        }, className: "embedded-table");

        return sw.ToString();
    }

    private static void WriteRow(TextWriter writer, string label)
    {
        HtmlBuilderRow.Data.With(writer,() =>
        {
            HtmlTagBuilderFactory.Data.Cell(writer, label);
        });
    }
}
