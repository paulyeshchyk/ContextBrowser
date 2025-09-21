using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextInfoKeyContainer<Key>
{
    public Key ContextKey { get; }
}
