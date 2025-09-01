using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextDataContainer
{
    string? Action { get; set; }

    HashSet<string> Domains { get; }
}
