using System.Collections.Generic;

namespace ContextBrowserKit.Matrix;

public interface IHtmlMatrix
{
    public List<string> rows { get; }

    public List<string> cols { get; }

    public IHtmlMatrix Transpose();
}

// context: model, htmlmatrix
// pattern: DTO
// parsing: error
public record HtmlMatrix : IHtmlMatrix
{
    public List<string> rows { get; }

    public List<string> cols { get; }

    public HtmlMatrix(List<string> rows, List<string> cols)
    {
        this.rows = rows;
        this.cols = cols;
    }

    // context: build, htmlmatrix
    public IHtmlMatrix Transpose()
    {
        return new HtmlMatrix(cols, rows);
    }
}