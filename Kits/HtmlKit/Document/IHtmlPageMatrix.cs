using System.Collections.Generic;
using ContextBrowserKit.Matrix;
using ContextKit.Model;
using ContextKit.Model.Collector;
using HtmlKit.Document.Coverage;

namespace HtmlKit.Document;

public interface IHtmlPageMatrix
{
    IContextInfoDataset<ContextInfo> Dataset { get; }

    IHtmlMatrix HtmlMatrix { get; }

    IContextInfoIndexerProvider FlatMapperProvider { get; }

    ICoverageManager CoverageManager { get; }

    string ProduceData(IContextKey container);
}