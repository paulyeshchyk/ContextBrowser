namespace ContextBrowser.Parser.csharp;

// Хранит состояние разбора одного файла
// context: model, file, csharp
public class ParseContext
{
    public string? Namespace;
    public ContextInfo? CurrentClass;
    public ContextInfo? CurrentMethod;
    public List<string> PendingContexts = new();
    public Dictionary<string, string> PendingDimensions = new();
    public List<ContextInfo> Result = new();
    public bool InsideMethod = false;
}