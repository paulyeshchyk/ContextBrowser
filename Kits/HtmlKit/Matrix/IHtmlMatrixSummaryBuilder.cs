using System.Collections.Generic;
using TensorKit.Model;

namespace HtmlKit.Matrix;

public interface IHtmlMatrixSummaryBuilder<TTensor>
    where TTensor : notnull
{
    HtmlMatrixSummary Build(IHtmlMatrix uiMatrix, TensorPermutationType orientation);

    Dictionary<object, int> ColsSummary(IHtmlMatrix uiMatrix, TensorPermutationType orientation);

    Dictionary<object, int> RowsSummary(IHtmlMatrix uiMatrix, TensorPermutationType orientation);
}
