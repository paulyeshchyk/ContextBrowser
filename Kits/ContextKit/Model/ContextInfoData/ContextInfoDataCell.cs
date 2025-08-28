namespace ContextKit.Model.Matrix;

public record struct ContextInfoDataCell(string Action, string Domain)
{
    public static implicit operator (string Action, string Domain)(ContextInfoDataCell value)
    {
        return (value.Action, value.Domain);
    }

    public static implicit operator ContextInfoDataCell((string Action, string Domain) value)
    {
        return new ContextInfoDataCell(value.Action, value.Domain);
    }
}