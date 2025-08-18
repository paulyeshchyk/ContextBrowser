namespace ContextKit.Model.Matrix;

public record struct ContextInfoMatrixCell(string Action, string Domain)
{
    public static implicit operator (string Action, string Domain)(ContextInfoMatrixCell value)
    {
        return (value.Action, value.Domain);
    }

    public static implicit operator ContextInfoMatrixCell((string Action, string Domain) value)
    {
        return new ContextInfoMatrixCell(value.Action, value.Domain);
    }
}