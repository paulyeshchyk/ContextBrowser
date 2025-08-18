using ContextBrowserKit.Matrix;
using ContextKit.Model;
using HtmlKit.Document.Coverage;

namespace HtmlKit.Document;

public interface IHtmlPageMatrix
{
    Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    UiMatrix UiMatrix { get; }

    Dictionary<string, ContextInfo> ContextsLookup { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(ContextContainer container);
}