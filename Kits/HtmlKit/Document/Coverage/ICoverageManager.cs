using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace HtmlKit.Document.Coverage;

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);

    string? BuildCellStyle(ContextInfoDataCell cell, List<ContextInfo>? methods, Dictionary<string, ContextInfo> contextLookup);
}