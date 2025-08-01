﻿namespace ContextKit.Model;

// context: model, ContextInfo
// pattern: Strategy
public class ContextClassifier : IContextClassifier
{
    public static string FakeAction => "_fakeAction";

    public static string FakeDomain => "_fakeDomain";

    public static string EmptyAction => "NoAction";

    public static string EmptyDomain => "NoDomain";

    /// <summary>
    /// Список предопределённых действий, определёющий есть у описания домен(ы) и действие, или нет.<br/>
    /// Если в описании есть только домен(ы), т.е. любые слова, но нет тех слов, что описаны в ContextClassifier.StandardActions, <br/>
    /// тогда описание будет трактоваться как с осутствующим действием.
    /// </summary>
    public readonly string[] StandardActions = new[] { "create", "read", "update", "delete", "validate", "share", "build", "model", "execute" };

    //используется для сортировки
    public static string LowestName => "zzz";

    public static readonly List<string> MetaItems = new List<string>() { "Action;Domain;Elements" };

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

    /// <summary>
    /// Определяет, существует ли домен и действие для объекта<br/>
    /// <br/>
    /// Пример: Результат будет true, потому что, есть домен(csharp) и есть действие (model)
    /// <code>
    /// // context: model, contextInfo
    /// public class ContextClassifier : IContextClassifier {}
    /// </code>
    /// <br/>
    /// Пример: Результат будет false, потому что, есть домен(csharp) и второй домен(dotnet), но нет действия
    /// <code>
    /// // context: dotnet, contextInfo
    /// public class ContextClassifier : IContextClassifier {}
    /// </code>
    /// <br/>
    /// Список предопределённых действий см: ContextBrowser.ContextKit.Parser.ContextClassifier.StandardActions
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool HasActionAndDomain(ContextInfo info)
    {
        if(info.Contexts == null)
            return false;

        var hasVerb = info.Contexts.Any(IsVerb);
        var hasNoun = info.Contexts.Any(IsNoun);

        return hasVerb && hasNoun;
    }
}

// context: model, contextInfo
public enum UnclassifiedPriority
{
    None,     // как сейчас
    Highest,  // NoAction / NoDomain идут первыми
    Lowest    // NoAction / NoDomain идут последними
}