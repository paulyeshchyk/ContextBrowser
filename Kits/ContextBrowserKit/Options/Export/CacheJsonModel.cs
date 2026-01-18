using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContextBrowserKit.Options.Export;

// context: roslyn, cache, model
public class CacheJsonModel
{
    public string Output { get; set; } = string.Empty;

    public string Input { get; set; } = string.Empty;

    public bool RenewCache { get; set; } = false;

    public CacheJsonModel()
    {
    }

    public CacheJsonModel(string? output, string? input, bool renewCache)
    {
        Output = output ?? string.Empty;
        Input = input ?? string.Empty;
        RenewCache = renewCache;
    }
}
