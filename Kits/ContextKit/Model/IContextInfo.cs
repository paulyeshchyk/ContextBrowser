using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextInfo : ISemanticInfo, ISpanInfo, ISemanticContainer
{
    IContextInfo? ClassOwner { get; set; }

    IContextInfo? MethodOwner { get; set; }

    string NameWithClassOwnerName { get; }
}

// context: IContextWithReferences, model
public interface IContextWithReferences<T> : IContextInfo, IContextDataContainerDomainPerAction
    where T : IContextWithReferences<T>
{
    // context: IContextWithReferences, read
    HashSet<T> References { get; }

    // context: IContextWithReferences, read
    HashSet<T> InvokedBy { get; }

    // context: IContextWithReferences, read
    HashSet<T> Properties { get; }

    // context: IContextWithReferences, read
    HashSet<T> Owns { get; }
}

public interface IContextDataContainerDomainPerAction
{
    string? Action { get; set; }

    HashSet<string> Domains { get; }
}