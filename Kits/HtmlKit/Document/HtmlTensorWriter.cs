using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
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
/// этих задач специализированным сервисам.
///
/// === Как это работает ===
/// 1. В конструктор HtmlMatrixWriter передаются все необходимые
///    компоненты:
///    - IHtmlDataCellBuilder<TKey>: отвечает за отрисовку содержимого
///      каждой ячейки, используя специфичные для TKey данные.
///    - DomainPerActionKeyFactory<TKey>: фабрика, которая знает, как создать
///      объект TKey из двух строк.
///    - DomainPerActionKeyBuilder: строитель, который определяет порядок
///      параметров для TKey в зависимости от ориентации матрицы.
///
/// 2. Класс IHtmlMatrixWriter<TKey> использует эти зависимости для
///    построения таблицы единообразно для любого TKey.
///
/// === Регистрация сервисов ===
/// Чтобы использовать HtmlMatrixWriter, необходимо зарегистрировать
/// все его зависимости в DI-контейнере.
///
/// Пример для базового ключа ContextKey:
///
/// hab.Services.AddTransient<IHtmlMatrixWriter, HtmlMatrixWriter<ContextKey>>();
/// hab.Services.AddTransient<DomainPerActionKeyFactory<ContextKey>>(provider =>
///     new ContextKeyFactory<ContextKey>((r, c) => new ContextKey(r, c)));
/// hab.Services.AddTransient<IHtmlDataCellBuilder<ContextKey>, HtmlDataCellBuilder<ContextKey>>();
/// hab.Services.AddTransient<DomainPerActionKeyBuilder, TensorBuilder>();
///
/// Пример для кастомного ключа DimensionKey:
///
/// hab.Services.AddTransient<IHtmlMatrixWriter, HtmlMatrixWriter<DimensionKey>>();
/// hab.Services.AddTransient<DomainPerActionKeyFactory<DimensionKey>>(provider =>
///     new ContextKeyFactory<DimensionKey>((r, c) => new DimensionKey(r, c)));
/// hab.Services.AddTransient<IHtmlDataCellBuilder<DimensionKey>, HtmlDataCellBuilder<DimensionKey>>();
/// hab.Services.AddTransient<DomainPerActionKeyBuilder, TensorBuilder>();
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
            await WriteHeaderRow(writer, matrix, options, cancellationToken).ConfigureAwait(false);
            await WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterFirst, cancellationToken).ConfigureAwait(false);
            await WriteAllDataRows(writer, matrix, summary, options, cancellationToken).ConfigureAwait(false);
            await WriteSummaryRowIf(writer, matrix, summary, options, SummaryPlacementType.AfterLast, cancellationToken).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    protected async Task WriteHeaderRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableRow.Meta.WithAsync(textWriter, async (token) =>
        {
            await WriteHeaderLeftCorner(textWriter, options, cancellationToken).ConfigureAwait(false);
            await WriteHeaderSummaryStart(textWriter, options, cancellationToken).ConfigureAwait(false);
            await WriteHeaderCols(textWriter, matrix, options, cancellationToken).ConfigureAwait(false);
            await WriteHeaderSummaryEnd(textWriter, options, cancellationToken).ConfigureAwait(false);
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
            HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.TopLeftCell(options), isEncodable: false);
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
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.FirstSummaryRow(options), isEncodable: false);
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
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, $"{col}", isEncodable: false);
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
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.LastSummaryRow(options), isEncodable: false);
                return Task.CompletedTask;
            }, cancellationToken).ConfigureAwait(false);
        }
    }

    internal async Task WriteSummaryRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        var total = summary?.ColsSummary?.Values.Sum() ?? 0;

        await HtmlBuilderFactory.HtmlBuilderTableRow.Summary.WithAsync(textWriter, async (token) =>
        {
            await HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.WithAsync(textWriter, (token) =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummaryAfterFirst(options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, _htmlFixedContentManager.SummaryRow(options), isEncodable: false);
                return Task.CompletedTask;
            }, token).ConfigureAwait(false);

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.WithAsync(textWriter, (token) =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, attrs, total.ToString(), isEncodable: false);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }

            foreach (var col in matrix.cols)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.WithAsync(textWriter, (token) =>
                {
                    var sum = summary?.ColsSummary?.GetValueOrDefault(col).ToString() ?? string.Empty;
                    var href = _hRefManager.GetHrefColSummary(col, options);
                    var colCellAttrs = new HtmlTagAttributes() { { "href", href } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, colCellAttrs, sum, isEncodable: false);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
            {
                await HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.WithAsync(textWriter, (token) =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(options) } };
                    HtmlBuilderFactory.A.CellAsync(textWriter, attrs, total.ToString(), isEncodable: false);
                    return Task.CompletedTask;
                }, token).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteDataRow(TextWriter textWriter, IHtmlMatrix matrix, HtmlTableOptions options, object row, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableRow.Data.WithAsync(textWriter, async (token) =>
        {
            var attrs = new HtmlTagAttributes();

            attrs.Add("class", "cell_row_meta center-aligned");

            await HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.WithAsync(textWriter, attributes: attrs, (token) =>
            {
                var href = _hRefManager.GetHRefRowHeader(row, options);
                var attrs = new HtmlTagAttributes() { { "href", href } };
                HtmlBuilderFactory.A.CellAsync(textWriter, attrs, $"{row}", isEncodable: false);
                return Task.CompletedTask;
            }, token).ConfigureAwait(false);

            if (options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                await WriteRowSummaryCell(textWriter, options, row, summary, token).ConfigureAwait(false);

            foreach (var col in matrix.cols)
            {
                var contextKey = _keyBuilder.BuildTensor(options.Orientation, new[] { row, col }, _keyFactory.Create);
                await _dataCellBuilder.BuildDataCell(textWriter, contextKey, options, token).ConfigureAwait(false);
            }

            if (options.SummaryPlacement == SummaryPlacementType.AfterLast)
                await WriteRowSummaryCell(textWriter, options, row, summary, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteRowSummaryCell(TextWriter textWriter, HtmlTableOptions options, object row, HtmlMatrixSummary? summary, CancellationToken cancellationToken)
    {
        await HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.WithAsync(textWriter, (token) =>
        {
            var rowSum = summary?.RowsSummary?.GetValueOrDefault(row).ToString() ?? string.Empty;
            var attributes = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowSummary(row, options) } };
            HtmlBuilderFactory.A.CellAsync(textWriter, attributes, rowSum, isEncodable: false);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }
}