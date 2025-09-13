using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Model;

public interface IHtmlCellStyleBuilder
{
    string? BuildCellStyle(IContextKey cell, IContextInfoDataset<ContextInfo> dataset, Dictionary<string, ContextInfo>? index);
}
