namespace ContextKit.Model;

public record ContextKey : IContextKey
{
    public string Action { get; set; }
    public string Domain { get; set; }

    public ContextKey(string Action, string Domain)
    {
        this.Action = Action;
        this.Domain = Domain;
    }
}