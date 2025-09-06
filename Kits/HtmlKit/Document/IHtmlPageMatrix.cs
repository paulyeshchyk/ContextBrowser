using System.Collections.Generic;
using ContextBrowserKit.Matrix;
using ContextKit.Model;
using HtmlKit.Document.Coverage;

namespace HtmlKit.Document;

public interface IHtmlPageMatrix
{
    IContextInfoDataset ContextsMatrix { get; }

    HtmlMatrix UiMatrix { get; }

    Dictionary<string, ContextInfo> IndexMap { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(IContextKey container);
}