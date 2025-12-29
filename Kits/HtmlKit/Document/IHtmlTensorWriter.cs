using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using HtmlKit.Matrix;

namespace HtmlKit.Document;

public interface IHtmlTensorWriter<TTensor>
    where TTensor : notnull
{
    Task WriteAsync(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options, CancellationToken cancellationToken);
}
