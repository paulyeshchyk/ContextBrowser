using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Document.Coverage;

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);

    string? BuildCellStyle(IContextKey cell, List<ContextInfo>? methods, Dictionary<string, ContextInfo> contextLookup);
}