using ContextBrowser.ContextCommentsParser;

namespace ContextBrowser.exporter;

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


public enum UnclassifiedPriority
{
    None,     // как сейчас
    Highest,  // NoAction / NoDomain идут первыми
    Lowest    // NoAction / NoDomain идут последними
}
