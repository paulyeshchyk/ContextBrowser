using System;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlMatrixGenerator
{
    Task<IHtmlMatrix> GenerateAsync(IDomainPerActionContextClassifier contextClassifier, TensorPermutationType matrixOrientation, UnclassifiedPriorityType priority, CancellationToken cancellationToken);
}
