namespace ContextKit.Model;

public interface IContextClassifier
{
    string EmptyAction { get; }

    string EmptyDomain { get; }

    string FakeAction { get; }

    string FakeDomain { get; }

    IEnumerable<string> MetaItems { get; }

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
    bool HasActionAndDomain(ContextInfo info);

    bool IsEmptyAction(string actionName);
    bool IsEmptyDomain(string domainName);

    bool IsActionApplicable(ContextInfo ctx, string? actionName);
    bool IsDomainApplicable(ContextInfo ctx, string? domainName);

    IEnumerable<string> GetCombinedVerbs(IEnumerable<string> verbs);
}