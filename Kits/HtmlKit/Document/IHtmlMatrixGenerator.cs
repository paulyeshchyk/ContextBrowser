using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Matrix;

namespace HtmlKit.Document;

public interface IHtmlMatrixGenerator
{
    Task<IHtmlMatrix> GenerateAsync(CancellationToken cancellationToken);
}
