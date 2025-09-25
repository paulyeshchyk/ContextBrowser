using System.IO;
using ContextBrowserKit.Options;

namespace HtmlKit.Document;

public interface IHtmlDataCellBuilder<TKey>
{
    void BuildDataCell(TextWriter textWriter, TKey key, HtmlTableOptions options);
}
