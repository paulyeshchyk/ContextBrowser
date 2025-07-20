using ContextBrowser.ContextCommentsParser;

namespace ContextBrowser.Parser;

// context: model, csharp
internal class ContextClassifier : IContextClassifier
{
    public readonly string[] StandardActions = new[] { "create", "read", "update", "delete", "validate", "share", "build", "model" };

    public static string EmptyDomain => "NoDomain";

    //используется для сортировки
    public static string LowestName => "zzz";

    public static string EmptyAction => "NoAction";

    public static readonly List<string> MetaItems = new List<string>() { "Action;Domain;Elements" };
    public bool IsNoun(string theWord) => !StandardActions.Contains(theWord);

    public bool IsVerb(string theWord) => StandardActions.Contains(theWord);
}

// context: model, csharp
internal static class ReservedWords
{
    public static readonly HashSet<string> CSharp = new HashSet<string> { "if", "for", "foreach", "while", "switch", "return", "await", "var", "let", "new", "typeof", "default", "nameof", "base", "this", "catch", "throw", "using", "true", "false", "null" };
}

public enum UnclassifiedPriority
{
    None,     // как сейчас
    Highest,  // NoAction / NoDomain идут первыми
    Lowest    // NoAction / NoDomain идут последними
}
