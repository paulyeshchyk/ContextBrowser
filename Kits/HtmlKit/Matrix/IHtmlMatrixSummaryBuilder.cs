using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace HtmlKit.Matrix;

public interface IHtmlMatrixSummaryBuilder<TTensor>
    where TTensor : notnull
{
    Task<HtmlMatrixSummary> BuildAsync(IHtmlMatrix uiMatrix, TensorPermutationType orientation, CancellationToken cancellationToken);
}
