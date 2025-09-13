using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

// Уровень изоляции - IHtmlMatrixWriter
// Определяет контракт для класса, который записывает HTML-матрицу.
// Это позволяет заменить реализацию без изменения клиентского кода.
public interface IHtmlMatrixWriter
{
    IHtmlMatrixWriter PrepareSummary();

    /// <summary>
    /// Записывает строку заголовка матрицы в TextWriter.
    /// </summary>
    /// <param name="textWriter">Объект для записи текста.</param>
    /// <returns>Текущий экземпляр HtmlMatrixWriter для цепочки вызовов.</returns>
    IHtmlMatrixWriter WriteHeaderRow(TextWriter textWriter);

    /// <summary>
    /// Записывает строку с итоговыми данными, если это необходимо.
    /// </summary>
    /// <param name="textWriter">Объект для записи текста.</param>
    /// <param name="placement">Тип размещения итоговой строки.</param>
    /// <returns>Текущий экземпляр HtmlMatrixWriter для цепочки вызовов.</returns>
    IHtmlMatrixWriter WriteSummaryRowIf(TextWriter textWriter, SummaryPlacementType placement);

    /// <summary>
    /// Записывает все строки данных матрицы.
    /// </summary>
    /// <param name="textWriter">Объект для записи текста.</param>
    /// <returns>Текущий экземпляр HtmlMatrixWriter для цепочки вызовов.</returns>
    IHtmlMatrixWriter WriteAllDataRows(TextWriter textWriter);
}

//context: htmlmatrix, build
internal class HtmlMatrixWriter : IHtmlMatrixWriter
{
    private readonly IHrefManager _hRefManager;
    private readonly IFixedHtmlContentManager _fixedHtmlContentManager;
    private readonly HtmlTableOptions _options;
    private readonly IHtmlMatrix _matrix;
    private readonly IHtmlDataCellBuilder<ContextKey> _dataCellBuilder;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IHtmlMatrixSummaryBuilder _summaryBuilder;

    private Dictionary<string, int>? _rowsSummary;
    private Dictionary<string, int>? _colsSummary;

    // Все зависимости, включая IHtmlMatrix, передаются через конструктор.
    public HtmlMatrixWriter(
        IHtmlDataCellBuilder<ContextKey> dataCellBuilder,
        IHrefManager hrefManager,
        IHtmlMatrix matrix,
        IFixedHtmlContentManager fixedHtmlContentManager,
        IHtmlMatrixSummaryBuilder summaryBuilder,
        IContextInfoDatasetProvider datasetProvider,
        HtmlTableOptions options)
    {
        _datasetProvider = datasetProvider;
        _hRefManager = hrefManager;
        _fixedHtmlContentManager = fixedHtmlContentManager;
        _matrix = matrix;
        _dataCellBuilder = dataCellBuilder;
        _summaryBuilder = summaryBuilder;
        _options = options;
    }

    public IHtmlMatrixWriter PrepareSummary()
    {
        var dataset = _datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();

        // Расчет итоговых данных, который также зависит от переданных сервисов.
        _rowsSummary = _summaryBuilder.RowsSummary(_matrix, dataset, _options.Orientation);
        _colsSummary = _summaryBuilder.ColsSummary(_matrix, dataset, _options.Orientation);

        return this;
    }

    public IHtmlMatrixWriter WriteHeaderRow(TextWriter textWriter)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Meta.With(textWriter, () =>
        {
            WriteHeaderLeftCorner(textWriter);
            WriteHeaderSummaryStart(textWriter);
            WriteHeaderCols(textWriter);
            WriteHeaderSummaryEnd(textWriter);
        });
        return this;
    }

    public IHtmlMatrixWriter WriteSummaryRowIf(TextWriter textWriter, SummaryPlacementType placement)
    {
        if (_options.SummaryPlacement == placement)
            WriteSummaryRow(textWriter);
        return this;
    }

    public IHtmlMatrixWriter WriteAllDataRows(TextWriter textWriter)
    {
        foreach (var row in _matrix.rows)
            WriteDataRow(textWriter, row);
        return this;
    }

    // Внутренние методы, которые не меняют бизнес-логику, но теперь используют
    // зависимости, переданные в конструкторе.
    internal void WriteHeaderLeftCorner(TextWriter textWriter)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.ActionDomain.With(textWriter, () =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) }, { "style", "some_special_cell_class" } };
            HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.TopLeftCell(_options), isEncodable: false);
        });
    }

    internal void WriteHeaderSummaryStart(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefColHeaderSummary(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.FirstSummaryRow(_options), isEncodable: false);
            });
        }
    }

    internal void WriteHeaderCols(TextWriter textWriter)
    {
        foreach (var col in _matrix.cols)
        {
            var href = _hRefManager.GetHRefRowMeta(col, _options);
            HtmlBuilderFactory.HtmlBuilderTableCell.ColMeta.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", href }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, col, isEncodable: false);
            });
        }
    }

    internal void WriteHeaderSummaryEnd(TextWriter textWriter)
    {
        if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummary(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.LastSummaryRow(_options), isEncodable: false);
            });
        }
    }

    internal void WriteSummaryRow(TextWriter textWriter)
    {
        var total = _colsSummary?.Values.Sum() ?? 0;

        HtmlBuilderFactory.HtmlBuilderTableRow.Summary.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.SummaryCaption.With(textWriter, () =>
            {
                var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowHeaderSummaryAfterFirst(_options) }, { "style", "some_special_cell_class" } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, _fixedHtmlContentManager.SummaryRow(_options), isEncodable: false);
            });

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }

            foreach (var col in _matrix.cols)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.ColSummary.With(textWriter, () =>
                {
                    var sum = _colsSummary?.GetValueOrDefault(col).ToString() ?? string.Empty;
                    var href = _hRefManager.GetHrefColSummary(col, _options);
                    var colCellAttrs = new HtmlTagAttributes() { { "href", href } };
                    HtmlBuilderFactory.A.Cell(textWriter, colCellAttrs, sum, isEncodable: false);
                });
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
            {
                HtmlBuilderFactory.HtmlBuilderTableCell.TotalSummary.With(textWriter, () =>
                {
                    var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefSummary(_options) } };
                    HtmlBuilderFactory.A.Cell(textWriter, attrs, total.ToString(), isEncodable: false);
                });
            }
        });
    }

    internal void WriteDataRow(TextWriter textWriter, string row)
    {
        HtmlBuilderFactory.HtmlBuilderTableRow.Data.With(textWriter, () =>
        {
            HtmlBuilderFactory.HtmlBuilderTableCell.RowMeta.With(textWriter, () =>
            {
                var href = _hRefManager.GetHRefRowHeader(row, _options);
                var attrs = new HtmlTagAttributes() { { "href", href } };
                HtmlBuilderFactory.A.Cell(textWriter, attrs, row, isEncodable: false);
            });

            if (_options.SummaryPlacement == SummaryPlacementType.AfterFirst)
                WriteRowSummaryCell(textWriter, row);

            foreach (var col in _matrix.cols)
            {
                var cell = _options.Orientation == MatrixOrientationType.ActionRows
                    ? new ContextKey(row, col)
                    : new ContextKey(col, row);

                _dataCellBuilder.BuildDataCell(textWriter, cell, _options);
            }

            if (_options.SummaryPlacement == SummaryPlacementType.AfterLast)
                WriteRowSummaryCell(textWriter, row);
        });
    }

    internal void WriteRowSummaryCell(TextWriter _tw, string row)
    {
        HtmlBuilderFactory.HtmlBuilderTableCell.RowSummary.With(_tw, () =>
        {
            var rowSum = _rowsSummary?.GetValueOrDefault(row).ToString() ?? string.Empty;
            var attributes = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefRowSummary(row, _options) } };
            HtmlBuilderFactory.A.Cell(_tw, attributes, rowSum, isEncodable: false);
        });
    }
}