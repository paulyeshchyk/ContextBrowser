using ContextKit.Model;

namespace ContextBrowser.Infrastructure;

// context: model, ContextInfo
// pattern: Strategy
// parsing: error
public record ContextClassifier : IContextClassifier
{
    public string EmptyAction { get; }

    public string EmptyDomain { get; }

    public string FakeAction { get; }

    public string FakeDomain { get; }

    /// <summary>
    /// Список предопределённых действий, определёющий есть у описания домен(ы) и действие, или нет.<br/>
    /// Если в описании есть только домен(ы), т.е. любые слова, но нет тех слов, что описаны в ContextClassifier.StandardActions, <br/>
    /// тогда описание будет трактоваться как с осутствующим действием.
    /// </summary>
    public IEnumerable<string> StandardActions { get; }

    public IEnumerable<string> MetaItems { get; }

    public ContextClassifier(string emptyAction, string emptyDomain, string fakeAction, string fakeDomain, string[] standardActions, string[] metaItems)
    {
        EmptyAction = emptyAction;
        EmptyDomain = emptyDomain;
        FakeAction = fakeAction;
        FakeDomain = fakeDomain;
        StandardActions = new List<string>(standardActions);
        MetaItems = new List<string>(metaItems);
    }

    public bool IsVerb(string theWord)
    {
        // Сначала проверяем, есть ли слово в основном списке
        if(StandardActions.Contains(theWord))
        {
            return true;
        }

        // Затем проверяем, не является ли оно специальным "фейковым" действием
        return theWord.Equals(FakeAction, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsNoun(string theWord)
    {
        return !IsVerb(theWord);
    }

    public bool HasActionAndDomain(ContextInfo info)
    {
        if(info.Contexts == null)
            return false;

        var hasVerb = info.Contexts.Any(IsVerb);
        var hasNoun = info.Contexts.Any(IsNoun);

        return hasVerb && hasNoun;
    }

    public IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs)
    {
        return verbs
            .Union(StandardActions)
            .ToList();
    }
}
