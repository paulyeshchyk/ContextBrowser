using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ContextKit.Model;

public interface IContextLinguisticClassifier
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
    bool IsVerb(string theWord);

    /// <summary>
    /// Определяет, является ли переданный контекст доменом<br/>
    /// </summary>
    /// <param name="theWord"></param>
    /// <returns></returns>
    bool IsNoun(string theWord);
}

public interface IContextTensorClassifier
{
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
    bool HasAllDimensionsFilled(ContextInfo info);
}

public interface IDomainPerActionEmptyDimensionClassifier
{
    string EmptyAction { get; }

    string EmptyDomain { get; }
}

public interface IDomainPerActionFakeDimensionClassifier
{
    string FakeAction { get; }

    string FakeDomain { get; }
}

public interface IDomainPerActionContextTensorClassifier : IContextTensorClassifier, IContextLinguisticClassifier, IDomainPerActionEmptyDimensionClassifier, IDomainPerActionFakeDimensionClassifier
{
    IEnumerable<string> MetaItems { get; }

    bool IsActionApplicable(ContextInfo ctx, string? actionName);

    bool IsDomainApplicable(ContextInfo ctx, string? domainName);
}

// context: model, ContextInfo
// pattern: Strategy
// parsing: error
public record DomainPerActionContextTensorClassifier : IDomainPerActionContextTensorClassifier
{
    public string EmptyAction { get; }

    public string EmptyDomain { get; }

    public string FakeAction { get; }

    public string FakeDomain { get; }

    public IEnumerable<string> StandardActions { get; }

    public IEnumerable<string> MetaItems { get; }

    public DomainPerActionContextTensorClassifier(string emptyAction, string emptyDomain, string fakeAction, string fakeDomain, string[] standardActions, string[] metaItems)
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
        if (StandardActions.Contains(theWord))
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

    public bool HasAllDimensionsFilled(ContextInfo info)
    {
        if (info.Contexts == null)
            return false;

        var hasVerb = info.Contexts.Any(IsVerb);
        var hasNoun = info.Contexts.Any(IsNoun);

        return hasVerb && hasNoun;
    }

    public IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs)
    {
        return verbs.Union(StandardActions).ToList();
    }

    public bool IsEmptyAction(string actionName)
    {
        return string.IsNullOrWhiteSpace(actionName) || actionName.Equals(EmptyAction);
    }

    public bool IsEmptyDomain(string domainName)
    {
        return string.IsNullOrWhiteSpace(domainName) || domainName.Equals(EmptyDomain);
    }

    public bool IsActionApplicable(ContextInfo ctx, string? actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName) || this.IsEmptyAction(actionName))
        {
            return string.IsNullOrEmpty(ctx.Action);
        }

        // Проверяем Action и классификатор
        return actionName.Equals(ctx.Action) && this.HasAllDimensionsFilled(ctx);
    }

    public bool IsDomainApplicable(ContextInfo ctx, string? domainName)
    {
        if (string.IsNullOrWhiteSpace(domainName) || this.IsEmptyDomain(domainName))
        {
            return ctx.Domains.Count() == 0;
        }

        // Проверяем домен и классификатор
        return ctx.Domains.Contains(domainName) && this.HasAllDimensionsFilled(ctx);
    }
}
