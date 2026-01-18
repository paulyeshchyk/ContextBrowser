using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: settings, model
public class ExportPathItem
{
    public ExportPathType Type { get; }

    public string Path { get; }

    public ExportPathItem(ExportPathType type, string path)
    {
        Type = type;
        Path = path;
    }
}