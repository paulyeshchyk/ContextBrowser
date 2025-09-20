using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Classifier;

public interface IWordRoleClassifier
{
    /// <summary>
    /// Список предопределённых действий, определёющий есть у описания домен(ы) и действие, или нет.<br/>
    /// Если в описании есть только домен(ы), т.е. любые слова, но нет тех слов, что описаны в ContextClassifier.StandardActions, <br/>
    /// тогда описание будет трактоваться как с осутствующим действием.
    /// </summary>
    IEnumerable<string> StandardActions { get; }

    /// <summary>
    /// Возвращает переданные действия вместе со стандартными
    /// </summary>
    /// <param name="verbs"></param>
    /// <returns></returns>
    IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs);

    /// <summary>
    /// Определяет, является ли переданный контекст действием<br/>
    /// </summary>
    /// <param name="theWord"></param>
    /// <returns></returns>
    bool IsVerb(string theWord, IFakeDimensionClassifier fakeDimensionClassifier);

    /// <summary>
    /// Определяет, является ли переданный контекст доменом<br/>
    /// </summary>
    /// <param name="theWord"></param>
    /// <returns></returns>
    bool IsNoun(string theWord, IFakeDimensionClassifier fakeDimensionClassifier);

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
    bool HasAllDimensionsFilled(ContextInfo info, IFakeDimensionClassifier fakeDimensionClassifier);
}

public record WordRoleClassifier : IWordRoleClassifier
{
    public IEnumerable<string> StandardActions { get; }

    public WordRoleClassifier(string[] standardActions)
    {
        StandardActions = standardActions;
    }

    public IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs)
    {
        return verbs.Union(StandardActions).ToList();
    }

    public bool IsNoun(string theWord, IFakeDimensionClassifier FakeDimensionClassifier)
    {
        return !IsVerb(theWord, FakeDimensionClassifier);
    }

    public bool IsVerb(string theWord, IFakeDimensionClassifier FakeDimensionClassifier)
    {
        // Сначала проверяем, есть ли слово в основном списке
        if (StandardActions.Contains(theWord))
        {
            return true;
        }

        // Затем проверяем, не является ли оно специальным "фейковым" действием
        return theWord.Equals(FakeDimensionClassifier.FakeAction, StringComparison.OrdinalIgnoreCase);
    }

    public bool HasAllDimensionsFilled(ContextInfo info, IFakeDimensionClassifier FakeDimensionClassifier)
    {
        if (info.Contexts == null)
            return false;

        var hasVerb = info.Contexts.Any(c => IsVerb(c, FakeDimensionClassifier));
        var hasNoun = info.Contexts.Any(c => IsNoun(c, FakeDimensionClassifier));

        return hasVerb && hasNoun;
    }
}