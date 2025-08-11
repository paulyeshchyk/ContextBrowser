namespace ContextBrowserKit.Matrix;

// context: model, matrix
// pattern: DTO
public record UiMatrix
{
    public List<string> rows = null!;
    public List<string> cols = null!;
}