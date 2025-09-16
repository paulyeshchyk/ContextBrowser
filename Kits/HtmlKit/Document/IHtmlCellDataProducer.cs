using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellDataProducer<TData>
{
    Task<TData> ProduceDataAsync(DomainPerActionTensor container, CancellationToken cancellationToken);
}
