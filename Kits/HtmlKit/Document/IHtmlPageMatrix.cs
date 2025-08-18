using ContextBrowserKit.Matrix;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using HtmlKit.Document.Coverage;

namespace HtmlKit.Document;

public interface IHtmlPageMatrix
{
    IContextInfoMatrix ContextsMatrix { get; }

    HtmlMatrix UiMatrix { get; }

    Dictionary<string, ContextInfo> ContextsLookup { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(ContextInfoMatrixCell container);
}