using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellDataProducer<TData, TKey>
    where TData : notnull
{
    Task<TData> ProduceDataAsync(TKey container, CancellationToken cancellationToken);
}
