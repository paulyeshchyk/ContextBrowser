using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfo, model
public interface IContextInfo : ISemanticInfo, ISpanInfo, ISemanticContainer
{
    IContextInfo? ClassOwner { get; set; }

    IContextInfo? MethodOwner { get; set; }

    string NameWithClassOwnerName { get; }

    /// <summary>
    /// индикатор того, что invocation не был корректно распарсен,
    /// если true то MethodOwner эквивалентен объекту
    /// однако, информацию о владельце можно взять из MethodOwner.Namespace & MethodOwner.ShortName
    /// </summary>
    bool MethodOwnedByItSelf { get; }
}

// context: ContextInfo, model
public interface IContextWithReferences<T> : IContextInfo, IContextDataContainerDomainPerAction
    where T : IContextWithReferences<T>
{
    // context: ContextInfo, read
    HashSet<T> References { get; }

    // context: ContextInfo, read
    HashSet<T> InvokedBy { get; }

    // context: ContextInfo, read
    HashSet<T> Properties { get; }

    // context: ContextInfo, read
    HashSet<T> Owns { get; }

    HashSet<string> Contexts { get; }

    Dictionary<string, string> Dimensions { get; }
}

public interface IContextDataContainerDomainPerAction
{
    string? Action { get; set; }

    HashSet<string> Domains { get; }

    void MergeDomains(IEnumerable<string> externalDomains);
}