using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Document;

public interface IHtmlMatrixGenerator
{
    Task<IHtmlMatrix> GenerateAsync(IContextClassifier contextClassifier, MatrixOrientationType matrixOrientation, UnclassifiedPriorityType priority, CancellationToken cancellationToken);
}
