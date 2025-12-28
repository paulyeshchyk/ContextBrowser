using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using HtmlKit.Builders.Page.CoHtmlElementBuilders;
using HtmlKit.Matrix;
using HtmlBuilderFactory = HtmlKit.Builders.Page.HtmlBuilderFactory;

namespace HtmlKit.Document;

//context: htmlmatrix, model
public interface IHtmlPageIndexProducer<TTensor>
    where TTensor : notnull
{
    Task<string> ProduceAsync(IHtmlMatrix matrix, CancellationToken cancellationToken);
}

//context: htmlmatrix, model
public class HtmlPageProducerIndex<TTensor> : HtmlPageProducer, IHtmlPageIndexProducer<TTensor>
    where TTensor : notnull
{
    private readonly IHtmlTensorWriter<TTensor> _matrixWriter;
    private readonly IHtmlMatrixSummaryBuilder<TTensor> _summaryBuilder;

    public HtmlPageProducerIndex(IHtmlTensorWriter<TTensor> matrixWriter, IHtmlMatrixSummaryBuilder<TTensor> summaryBuilder, IAppOptionsStore optionsStore) : base(optionsStore)
    {
        _matrixWriter = matrixWriter;
        _summaryBuilder = summaryBuilder;
    }

    public async Task<string> ProduceAsync(IHtmlMatrix matrix, CancellationToken cancellationToken)
    {
        await using var sw = new StringWriter();
        await ProduceAsync(sw, matrix, cancellationToken).ConfigureAwait(false);

        var result = sw.ToString();
        return result;
    }

    protected override IEnumerable<string> GetAdditionalScripts()
    {
        yield return Resources.ScriptAutoShrinkEmbeddedTable;
        yield return Resources.ScriptAutoFontShrink;
    }

    protected override async Task WriteContentAsync(TextWriter writer, IHtmlMatrix matrix, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var navigationItem = new BreadcrumbNavigationItem($"..\\index.html?v={timeStamp}", "Контекстная матрица");
        await HtmlBuilderFactory.Breadcrumb(navigationItem).CellAsync(writer, cancellationToken: cancellationToken).ConfigureAwait(false);

        await HtmlBuilderFactory.P.CellAsync(writer, cancellationToken: cancellationToken).ConfigureAwait(false);

        var summary = await _summaryBuilder.BuildAsync(matrix, options.Orientation, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _matrixWriter.WriteAsync(writer, matrix, summary, options, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}