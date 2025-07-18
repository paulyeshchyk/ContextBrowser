namespace ContextBrowser.Parser;

public record struct ContextContainer(string Action, string Domain)
{
    public static implicit operator (string Action, string Domain)(ContextContainer value)
    {
        return (value.Action, value.Domain);
    }

    public static implicit operator ContextContainer((string Action, string Domain) value)
    {
        return new ContextContainer(value.Action, value.Domain);
    }
}