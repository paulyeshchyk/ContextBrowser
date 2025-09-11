using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Extensions;

public interface IUiMatrixSummaryBuilder
{
    Dictionary<string, int>? ColsSummary(IHtmlMatrix uiMatrix, IContextInfoDataset<ContextInfo> matrix, MatrixOrientationType orientation);

    Dictionary<string, int>? RowsSummary(IHtmlMatrix uiMatrix, IContextInfoDataset<ContextInfo> matrix, MatrixOrientationType orientation);
}
