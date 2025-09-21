using System.Collections.Generic;

namespace HtmlKit.Matrix;

// context: model, htmlmatrix
// pattern: DTO
// parsing: error
public record HtmlMatrixDomainPerAction : IHtmlMatrix
{
    public List<object> rows { get; }

    public List<object> cols { get; }

    public HtmlMatrixDomainPerAction(List<object> rows, List<object> cols)
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
