using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.Parser;

// context: model, csharp
// pattern: Strategy
public class ContextClassifier : IContextClassifier
{
    public readonly string[] StandardActions = new[] { "create", "read", "update", "delete", "validate", "share", "build", "model" };

    public static string EmptyDomain => "NoDomain";

    //используется для сортировки
    public static string LowestName => "zzz";

    public static string EmptyAction => "NoAction";

    public static readonly List<string> MetaItems = new List<string>() { "Action;Domain;Elements" };
    public bool IsNoun(string theWord) => !StandardActions.Contains(theWord);

    public bool IsVerb(string theWord) => StandardActions.Contains(theWord);

    public bool HasActionAndDomain(ContextInfo info)
    {
        if(info.Contexts == null)
            return false;

        var hasVerb = info.Contexts.Any(IsVerb);
        var hasNoun = info.Contexts.Any(IsNoun);

        return hasVerb && hasNoun;
    }
}


// context: model, csharp
public enum UnclassifiedPriority
{
    None,     // как сейчас
    Highest,  // NoAction / NoDomain идут первыми
    Lowest    // NoAction / NoDomain идут последними
}
