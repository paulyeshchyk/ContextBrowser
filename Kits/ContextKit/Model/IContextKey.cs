using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

public interface IContextKey
{
    string Action { get; set; }

    string Domain { get; set; }
}

