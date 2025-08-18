namespace ContextBrowserKit.Matrix;

// context: model, htmlmatrix
// pattern: DTO
// parsing: error
public record HtmlMatrix
{
    public readonly List<string> rows;
    public readonly List<string> cols;

    public HtmlMatrix(List<string> rows, List<string> cols)
    {
        this.rows = rows;
        this.cols = cols;
    }

    // context: build, htmlmatrix
    public HtmlMatrix Transpose()
    {
        return new HtmlMatrix(cols, rows);
    }
}