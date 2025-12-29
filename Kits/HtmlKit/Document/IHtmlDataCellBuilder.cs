using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;

namespace HtmlKit.Document;

public interface IHtmlDataCellBuilder<TKey>
{
    Task BuildDataCell(TextWriter textWriter, TKey key, HtmlTableOptions options, CancellationToken cancellationToken);
}
