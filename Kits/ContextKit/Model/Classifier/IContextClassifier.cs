using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Classifier;

public interface IContextClassifier<TContext>
    where TContext : IContextWithReferences<TContext>
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
    /// <param name="fakeDimensionClassifier"></param>
    /// <returns></returns>
    bool IsVerb(string theWord, IFakeDimensionClassifier fakeDimensionClassifier);

    /// <summary>
    /// Определяет, является ли переданный контекст доменом<br/>
    /// </summary>
    /// <param name="theWord"></param>
    /// <param name="fakeDimensionClassifier"></param>
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
    /// <param name="fakeDimensionClassifier"></param>
    /// <returns></returns>
    bool HasAllDimensionsFilled(TContext info, IFakeDimensionClassifier fakeDimensionClassifier);
}

public record ContextClassifier : IContextClassifier<ContextInfo>
{
    public IEnumerable<string> StandardActions { get; }

    public ContextClassifier(string[] standardActions)
    {
        StandardActions = standardActions;
    }

    public IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs)
    {
        return verbs.Union(StandardActions).ToList();
    }

    public bool IsNoun(string theWord, IFakeDimensionClassifier fakeDimensionClassifier)
    {
        // Сначала проверяем, является ли оно специальным "фейковым" доменом
        // Затем проверяем,  не является ли действием
        return theWord.Equals(fakeDimensionClassifier.FakeDomain, StringComparison.OrdinalIgnoreCase) || !IsVerb(theWord, fakeDimensionClassifier);
    }

    public bool IsVerb(string theWord, IFakeDimensionClassifier fakeDimensionClassifier)
    {
        // Сначала проверяем, является ли оно специальным "фейковым" действием
        // Затем проверяем, есть ли слово в основном списке
        return theWord.Equals(fakeDimensionClassifier.FakeAction, StringComparison.OrdinalIgnoreCase) || StandardActions.Contains(theWord);
    }

    public bool HasAllDimensionsFilled(ContextInfo info, IFakeDimensionClassifier fakeDimensionClassifier)
    {
        if (!info.Contexts.Any())
            return false;

        var hasVerb = info.Contexts.Any(c => IsVerb(c, fakeDimensionClassifier));
        var hasNoun = info.Contexts.Any(c => IsNoun(c, fakeDimensionClassifier));

        return hasVerb && hasNoun;
    }
}