using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Matrix;

// context: model, htmlmatrix
// pattern: DTO
// parsing: error
public record HtmlMatrixDomainPerAction : IHtmlMatrix
{
    public List<ILabeledValue> rows { get; }

    public List<ILabeledValue> cols { get; }

    public HtmlMatrixDomainPerAction(List<ILabeledValue> rows, List<ILabeledValue> cols)
    {
        this.rows = rows;
        this.cols = cols;
    }

    // context: build, htmlmatrix
    public IHtmlMatrix Transpose()
    {
        return new HtmlMatrixDomainPerAction(cols, rows);
    }
}
