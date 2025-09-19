using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
    Task<string> ProduceAsync(IHtmlMatrix matrix, CancellationToken cancellationToken);
}

//context: htmlmatrix, model
public class HtmlPageProducerIndex : HtmlPageProducer, IHtmlPageIndex
{
    private readonly IHtmlMatrixWriter _matrixWriter;
    private readonly IHtmlMatrixSummaryBuilder _summaryBuilder;

    public HtmlPageProducerIndex(IHtmlMatrixWriter matrixWriter, IHtmlMatrixSummaryBuilder summaryBuilder, IAppOptionsStore optionsStore) : base(optionsStore)
    {
        _matrixWriter = matrixWriter;
        _summaryBuilder = summaryBuilder;
    }

    public Task<string> ProduceAsync(IHtmlMatrix matrix, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            using var sw = new StringWriter();
            Produce(sw, matrix);

            var result = sw.ToString();
            return result;
        }, cancellationToken);
    }

    protected override IEnumerable<string> GetAdditionalScripts()
    {
        yield return Resources.ScriptAutoShrinkEmbeddedTable;
        yield return Resources.ScriptAutoFontShrink;
    }

    protected override void WriteContent(TextWriter writer, IHtmlMatrix matrix, HtmlTableOptions options)
    {
        var navigationItem = new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица");
        HtmlBuilderFactory.Breadcrumb(navigationItem).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        var summary = _summaryBuilder.Build(matrix, options.Orientation);

        _matrixWriter.Write(writer, matrix, summary, options);
    }
}