using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

//context: htmlmatrix, model
public interface IHtmlPageIndex
{
    string Produce(IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options);
}

//context: htmlmatrix, model
public class HtmlPageProducerIndex : HtmlPageProducer, IHtmlPageIndex
{
    private readonly IHtmlMatrixWriter _matrixWriter;

    //private readonly IHtmlMatrixSummaryBuilder _htmlMatrixSummaryBuilder;

    public HtmlPageProducerIndex(IHtmlMatrixWriter matrixWriter) : base()
    {
        _matrixWriter = matrixWriter;
    }

    public string Produce(IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options)
    {
        using var sw = new StringWriter();
        Produce(sw, matrix, summary, options);
        return sw.ToString();
    }

    protected override IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript;
        yield return Resources.ScriptAutoShrinkEmbeddedTable;
        yield return Resources.ScriptAutoFontShrink;
    }

    protected override void WriteContent(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary summary, HtmlTableOptions options)
    {
        var navigationItem = new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица");
        HtmlBuilderFactory.Breadcrumb(navigationItem).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        _matrixWriter.Write(writer, matrix, summary, options);
    }
}
