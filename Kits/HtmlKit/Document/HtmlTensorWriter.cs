using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Helpers;
using HtmlKit.Matrix;
using LoggerKit;
using TensorKit.Factories;
using TensorKit.Model;

namespace HtmlKit.Document;

/// <summary>
/// HtmlMatrixWriter - это универсальный сервис для построения HTML-таблиц.
///
/// Этот класс работает с любым типом ключа (TKey), который реализует
/// необходимые зависимости. Он не содержит логики создания ключей или
/// отображения данных; вместо этого, он использует принцип композиции
/// и внедрение зависимостей (Dependency Injection) для делегирования
/// этих задач специализированным сервисам.<br/>
///<br/>
/// === Как это работает ===<br/>
/// 1. В конструктор HtmlMatrixWriter передаются все необходимые<br/>
///    компоненты:<br/>
///    - IHtmlDataCellBuilder&lt;TKey&gt;: отвечает за отрисовку содержимого<br/>
///      каждой ячейки, используя специфичные для TKey данные.<br/>
///    - DomainPerActionKeyFactory&lt;Key&gt;: фабрика, которая знает, как создать<br/>
///      объект TKey из двух строк.<br/>
///    - DomainPerActionKeyBuilder: строитель, который определяет порядок<br/>
///      параметров для TKey в зависимости от ориентации матрицы.<br/>
///<br/>
/// 2. Класс IHtmlMatrixWriter&lt;TKey&gt; использует эти зависимости для<br/>
///    построения таблицы единообразно для любого TKey.<br/>
///<br/>
/// === Регистрация сервисов ===<br/>
/// Чтобы использовать HtmlMatrixWriter, необходимо зарегистрировать<br/>
/// все его зависимости в DI-контейнере.<br/>
///<br/>
/// Пример для базового ключа ContextKey:<br/>
///<br/>
/// hab.Services.AddTransient&lt;IHtmlMatrixWriter, HtmlMatrixWriter&lt;ContextKey&gt;&gt;();<br/>
/// hab.Services.AddTransient&lt;DomainPerActionKeyFactory&lt;ContextKey&gt;&gt;(provider =><br/>
///     new ContextKeyFactory&lt;ContextKey&gt;((r, c) => new ContextKey(r, c)));<br/>
/// hab.Services.AddTransient&lt;IHtmlDataCellBuilder&lt;ContextKey&gt;, HtmlDataCellBuilder&lt;ContextKey&gt;&gt;();<br/>
/// hab.Services.AddTransient&lt;DomainPerActionKeyBuilder, TensorBuilder&gt;();<br/>
///<br/>
/// Пример для кастомного ключа DimensionKey:<br/>
///<br/>
/// hab.Services.AddTransient&lt;IHtmlMatrixWriter, HtmlMatrixWriter&lt;DimensionKey&gt;&gt;();<br/>
/// hab.Services.AddTransient&lt;DomainPerActionKeyFactory&lt;DimensionKey&gt;&gt;(provider =><br/>
///     new ContextKeyFactory&lt;DimensionKey&gt;((r, c) => new DimensionKey(r, c)));<br/>
/// hab.Services.AddTransient&lt;IHtmlDataCellBuilder&lt;DimensionKey&gt;, HtmlDataCellBuilder&lt;DimensionKey&gt;&gt;();<br/>
/// hab.Services.AddTransient&lt;DomainPerActionKeyBuilder, TensorBuilder&gt;();<br/>
///
/// </summary>
/// <typeparam name="TTensor">Тип ключа для ячейки матрицы. Должен иметь конструктор с двумя строковыми параметрами.</typeparam>
//context: htmlmatrix, build
public class HtmlTensorWriter<TTensor> : IHtmlTensorWriter<TTensor>
    where TTensor : ITensor
{
    private readonly IHtmlHrefManager<TTensor> _hRefManager;
    private readonly IHtmlFixedContentManager _htmlFixedContentManager;
    private readonly IHtmlDataCellBuilder<TTensor> _dataCellBuilder;
    private readonly ITensorFactory<TTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlTensorWriter(
        IHtmlDataCellBuilder<TTensor> dataCellBuilder,
        IHtmlHrefManager<TTensor> hrefManager,
        ITensorFactory<TTensor> keyFactory,
        ITensorBuilder keyBuilder,
        IHtmlFixedContentManager htmlFixedContentManager,
        IAppLogger<AppLevel> logger)
    {
        _hRefManager = hrefManager;
        _htmlFixedContentManager = htmlFixedContentManager;
        _dataCellBuilder = dataCellBuilder;
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
        _logger = logger;
    }

    public async Task WriteAsync(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions originalOptions, CancellationToken cancellationToken)
    {
        //если summary не пришло, то просто говорим, что summary рисовать не нужно
        var options = summary == null ? originalOptions with { SummaryPlacement = SummaryPlacementType.None } : originalOptions;

        await HtmlBuilderFactory.Table.WithAsync(writer, async (token) =>
        {
            await WriteHeaderRow(writer, matrix, options, token).ConfigureAwait(false);
            await WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterFirst, token).ConfigureAwait(false);
            await WriteAllDataRows(writer, matrix, summary, options, token).ConfigureAwait(false);
            await WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterLast, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    protected async Task WriteHeaderRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableRow.Meta.WithAsync(textWriter, async (token) =>
        {
            await WriteHeaderLeftCorner(textWriter, options, token).ConfigureAwait(false);
            await WriteHeaderSummaryStart(textWriter, options, token).ConfigureAwait(false);
            await WriteHeaderCols(textWriter, matrix, options, token).ConfigureAwait(false);
            await WriteHeaderSummaryEnd(textWriter, options, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    protected async Task WriteSummaryRowIf(TextWriter textWriter, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options, SummaryPlacementType placement, CancellationToken cancellationToken)
    {
        if (options.SummaryPlacement == placement)
        {
            await WriteSummaryRow(textWriter, matrix, options, summary, cancellationToken).ConfigureAwait(false);
        }
    }

    protected async Task WriteAllDataRows(TextWriter textWriter, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        foreach (var row in matrix.rows)
        {
            await WriteDataRow(textWriter, matrix, options, row, summary, cancellationToken).ConfigureAwait(false);
        }
    }

    // Внутренние методы, которые не меняют бизнес-логику, но теперь используют
    // зависимости, переданные в конструкторе.
    internal async Task WriteHeaderLeftCorner(TextWriter textWriter, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableCell.ActionDomain.WithAsync(textWriter, (token) =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) }, { "style", "some_special_cell_class" } };
            HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.TopLeftCell(options), isEncodable: false, cancellationToken: token);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteHeaderSummaryStart(TextWriter textWriter, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
        {
            await HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.WithAsync(textWriter, (token) =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefColHeaderSummary(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.FirstSummaryRow(options), isEncodable: false, cancellationToken: token);
                return Task.CompletedTask;
            }, cancellationToken).ConfigureAwait(false);
        }
    }

    internal async Task WriteHeaderCols(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        foreach (var col in matrix.cols)
        {
            var href = _hRefManager.GetHRefRowMeta(col, options);
            await HtmlBuilderFactory.HtmlBuilderTableCell.ColMeta.WithAsync(textWriter, (token) =>
            {
                var attrs = new HtmlTagAttributes() { { "href", href }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, col.LabeledCaption, isEncodable: false, cancellationToken: token);
                return Task.CompletedTask;
            }, cancellationToken).ConfigureAwait(false);
        }
    }

    internal async Task WriteHeaderSummaryEnd(TextWriter textWriter, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
        {
            await HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.WithAsync(textWriter, (token) =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummary(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.LastSummaryRow(options), isEncodable: false, cancellationToken: token);
                return Task.CompletedTask;
            }, cancellationToken).ConfigureAwait(false);
        }
    }

    internal async Task WriteSummaryRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        var total = summary?.ColsSummary.Values.Sum() ?? 0;

        await HtmlBuilderFactory.HtmlBuilderTableRow.Summary.WithAsync(textWriter, async (token) =>
        {
            await HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.WithAsync(textWriter, (atoken) =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummaryAfterFirst(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.SummaryRow(options), isEncodable: false, cancellationToken: atoken);
                return Task.CompletedTask;
            }, token).ConfigureAwait(false);

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.WithAsync(textWriter, (atoken) =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, attrs, total.ToString(), isEncodable: false, cancellationToken: atoken);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }

            foreach (var col in matrix.cols)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.WithAsync(textWriter, (atoken) =>
                {
                    var sum = summary?.ColsSummary.GetValueOrDefault(col.LabeledData).ToString() ?? string.Empty;
                    var href = _hRefManager.GetHrefColSummary(col, options);
                    var tagAttrs = new HtmlTagAttributes() { { "href", href } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, tagAttrs, sum, isEncodable: false, cancellationToken: atoken);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.WithAsync(textWriter, (atoken) =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, attrs, total.ToString(), isEncodable: false, cancellationToken: atoken);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteDataRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, ILabeledValue row, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableRow.Data.WithAsync(textWriter, async (token) =>
        {
            var attrs = new HtmlTagAttributes { { "class", "cell_row_meta center-aligned" } };

            await HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.WithAsync(textWriter, attributes: attrs, (atoken) =>
            {
                var href = _hRefManager.GetHRefRowHeader(row, options);
                var metaattrs = new HtmlTagAttributes() { { "href", href } };
                HtmlBuilderFactory.A.CellAsync(textWriter, metaattrs, $"{row}", isEncodable: false, cancellationToken: atoken);
                return Task.CompletedTask;
            }, token).ConfigureAwait(false);

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                await WriteRowSummaryCell(textWriter, options, row, summary, token).ConfigureAwait(false);

            foreach (var col in matrix.cols)
            {
                var contextKey = _keyBuilder.BuildTensor(options.Orientation, new[] { row.LabeledData, col.LabeledData }, _keyFactory.Create);
                await _dataCellBuilder.BuildDataCell(textWriter, contextKey, options, token).ConfigureAwait(false);
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
                await WriteRowSummaryCell(textWriter, options, row, summary, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteRowSummaryCell(TextWriter textWriter, HtmlTableOptions options, ILabeledValue row, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.WithAsync(textWriter, (token) =>
        {
            var rowSum = summary?.RowsSummary.GetValueOrDefault(row.LabeledData).ToString() ?? string.Empty;
            var tagAttrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowSummary(row, options) } };
            HtmlBuilderFactory.A.CellAsync(textWriter, tagAttrs, rowSum, isEncodable: false, cancellationToken: token);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }
}