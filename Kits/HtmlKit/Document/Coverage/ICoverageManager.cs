using ContextKit.Model;

namespace HtmlKit.Document.Coverage;

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);

    string? BuildCellStyle(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup);
}