using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace HtmlKit.Document.Coverage;

public interface IHtmlCellStyleBuilder
{
    string? BuildCellStyle(IContextKey cell, IContextInfoDataset<ContextInfo> dataset, Dictionary<string, ContextInfo>? index);
}
