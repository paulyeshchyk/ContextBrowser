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