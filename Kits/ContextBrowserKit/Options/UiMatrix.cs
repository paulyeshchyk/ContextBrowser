namespace ContextBrowserKit.Matrix;

// context: model, matrix
// pattern: DTO
// parsing: error
public record UiMatrix
{
    public List<string> rows = null!;
    public List<string> cols = null!;
}