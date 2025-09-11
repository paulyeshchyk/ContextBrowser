using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace HtmlKit.Document.Coverage;

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);

    string? BuildCellStyle(IContextKey cell, List<ContextInfo>? methods, IHtmlMatrixIndexer<ContextInfo> contextLookup);
}