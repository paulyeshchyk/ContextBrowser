using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Matrix;

namespace HtmlKit.Document;

public interface IHtmlMatrixGenerator
{
    Task<IHtmlMatrix> GenerateAsync(CancellationToken cancellationToken);
}
