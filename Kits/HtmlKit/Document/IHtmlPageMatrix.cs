using System.Collections.Generic;
using ContextBrowserKit.Matrix;
using ContextKit.Model;
using ContextKit.Model.Collector;
using HtmlKit.Document.Coverage;

namespace HtmlKit.Document;

public interface IHtmlPageDataProducer
{
    string ProduceData(IContextKey container);
}

public interface IHtmlPageMatrix : IHtmlPageDataProducer
{
    IHtmlMatrix HtmlMatrix { get; }
}