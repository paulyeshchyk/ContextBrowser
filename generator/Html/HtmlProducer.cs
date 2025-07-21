using ContextBrowser.exporter;
using ContextBrowser.Generator.Matrix;
using ContextBrowser.model;
using System.Text;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

internal class HtmlProducer
{
    private readonly StringBuilder sb = new StringBuilder();
    public readonly HtmlTableOptions Options;
    private readonly Dictionary<string, ContextInfo> contextLookup;

    public HtmlProducer(HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup)
    {
        Options = options;
        this.contextLookup = contextLookup;
    }

    public string GetResult() => sb.ToString();

    public void ProduceHtmlStart() => sb.AppendLine("<!DOCTYPE html><html>");

    public void ProduceHead()
    {
        sb.AppendLine("<head><meta charset=\"UTF-8\"><title>📦 Контекстная матрица</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(Resources.HtmlProducerContentStyle);
        sb.AppendLine("</style>");
        sb.AppendLine("<script>");
        sb.AppendLine(Resources.HtmlProducerContentStyleScript);
        sb.AppendLine("</script>");
        sb.AppendLine("</head>");
    }

    public void ProduceHtmlEnd() => sb.AppendLine("</html>");

    public void ProduceHtmlBodyStart() => sb.AppendLine("<body>");

    public void ProduceHtmlBodyEnd() => sb.AppendLine("</body>");

    public void ProduceTitle() => sb.AppendLine("<h1>📦 Навигация по архитектурным зонам</h1>");

    public void ProduceTableStart() => sb.AppendLine("<table>");

    public void ProduceTableEnd() => sb.AppendLine("</table>");

    public void ProduceTableHeaderRow(UiMatrix uiMatrix)
    {
        HtmlRowBuilder.Meta.Start(sb);
        ProduceTableHeaderFirstCellContent();
        ProduceTableFirstRowCellSummary();
        ProduceTableHeaderRowCells(uiMatrix);
        ProduceTableLastRowCellSummary();
        HtmlRowBuilder.Meta.End(sb);
    }

    public void ProduceColumnSummaryRow(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var colSums = uiMatrix.ColsSummary(matrix, Options.Orientation);
        var totalSum = colSums?.Values.Sum() ?? 0;

        HtmlRowBuilder.Summary.Start(sb);
        HtmlCellBuilder.SummaryCaption.Cell(sb, "<b>Σ</b>");

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            HtmlCellBuilder.TotalSummary.Cell(sb, totalSum.ToString(), "index.html");

        ProduceColumnSummaryCells(uiMatrix, colSums);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            HtmlCellBuilder.TotalSummary.Cell(sb, totalSum.ToString(), "index.html");

        HtmlRowBuilder.Summary.End(sb);
    }

    public static string? GetCoverageColorForCell(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup, Func<ContextInfo?, int> DimensionValueExtractor)
    {
        if(methods != null && methods.Count > 0)
        {
            var covs = methods
                .Select(name => contextLookup.TryGetValue(name, out var ctx)
                    ? DimensionValueExtractor(ctx)
                    : 0)
                .ToList();

            if(covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        if(contextLookup.TryGetValue(cell.Action, out var actionCtx))
        {
            var aVal = DimensionValueExtractor(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        if(contextLookup.TryGetValue(cell.Domain, out var domainCtx))
        {
            var aVal = DimensionValueExtractor(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }

    public void ProduceDataRow(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        HtmlRowBuilder.Data.Start(sb);
        ProduceRowHeaderCell(row);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceRowSummaryCell(row, uiMatrix, matrix);

        ProduceDataCells(row, uiMatrix, matrix);
        ProduceRowSummaryCellLast(row, uiMatrix, matrix);

        HtmlRowBuilder.Data.End(sb);
    }

    public void ProduceMatrix(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        ProduceTableHeaderRow(uiMatrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceColumnSummaryRow(uiMatrix, matrix);

        foreach(var row in uiMatrix.rows)
            ProduceDataRow(row, uiMatrix, matrix);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            ProduceColumnSummaryRow(uiMatrix, matrix);
    }

    private void ProduceTableHeaderFirstCellContent()
    {
        var theText = Options.Orientation == MatrixOrientation.ActionRows
            ? "Action \\ Domain"
            : "Domain \\ Action";
        HtmlCellBuilder.ActionDomain.Cell(sb, theText);
    }

    private void ProduceTableFirstRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            HtmlCellBuilder.SummaryCaption.Start(sb);
    }

    private void ProduceTableLastRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            HtmlCellBuilder.SummaryCaption.Start(sb);
    }

    private void ProduceTableHeaderRowCells(UiMatrix uiMatrix)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var hRef = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            HtmlCellBuilder.ColMeta.Cell(sb, metaName, hRef);
        }
    }

    private void ProduceRowHeaderCell(string metaName)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{metaName}.html"
            : $"domain_{metaName}.html";

        HtmlCellBuilder.RowMeta.Cell(sb, metaName, hRef);
    }

    private void ProduceColumnSummaryCells(UiMatrix uiMatrix, Dictionary<string, int>? colSums)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var sum = colSums?[metaName];
            var href = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            HtmlCellBuilder.ColSummary.Cell(sb, sum?.ToString() ?? string.Empty, href);
        }
    }

    private void ProduceRowSummaryCell(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        HtmlCellBuilder.RowSummary.Cell(sb, rowSum?.ToString() ?? string.Empty, hRef);
    }

    private static int GetCoverageValue(ContextInfo? ctx)
    {
        return ctx?.GetDimensionIntValue("coverage") ?? 0;
    }

    private void ProduceDataCells(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        foreach(var col in uiMatrix.cols)
        {
            var cell = Options.Orientation == MatrixOrientation.ActionRows
                ? new ContextContainer(row, col)
                : new ContextContainer(col, row);

            var hasMethods = matrix.TryGetValue(cell, out var methods) && methods.Any();
            var hRef = $"composite_{cell.Action}_{cell.Domain}.html";

            string style = string.Empty;
            var bgColor = GetCoverageColorForCell(cell, hasMethods ? methods : null, contextLookup, GetCoverageValue);
            if(bgColor != null)
            {
                style = $" style=\"inherited;background-color:{bgColor}; color:black\"";
            }
            HtmlCellBuilder.Data.Cell(sb, $"{(hasMethods ? methods?.Count ?? 0 : "&nbsp;")}", hRef, style);
        }
    }

    private void ProduceRowSummaryCellLast(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        if(Options.SummaryPlacement != SummaryPlacement.AfterLast)
            return;

        var rowFile = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        HtmlCellBuilder.RowSummary.Cell(sb, rowSum?.ToString() ?? string.Empty, rowFile);
    }
}

internal class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}
