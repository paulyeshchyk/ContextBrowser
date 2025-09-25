using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Document;

public interface IHtmlCellDataProducer<TData, TKey>
    where TData : notnull
{
    Task<TData> ProduceDataAsync(TKey container, CancellationToken cancellationToken);
}
