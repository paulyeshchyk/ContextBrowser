using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.HtmlKit.Document.Coverage;
using ContextBrowser.HtmlKit.Model;

namespace ContextBrowser.HtmlKit.Document;

public interface IHtmlPageMatrix
{
    Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    UiMatrix UiMatrix { get; }

    Dictionary<string, ContextInfo> ContextsLookup { get; }

    HtmlTableOptions Options { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(ContextContainer container);
}
