using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellDataProducer<TData>
{
    Task<TData> ProduceDataAsync(IContextKey container, CancellationToken cancellationToken);
}
