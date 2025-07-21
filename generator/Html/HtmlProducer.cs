using ContextBrowser.exporter;
using ContextBrowser.Generator.Matrix;
using ContextBrowser.model;
using System.Text;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

internal static class HtmlClasses
{
    public static class Row
    {
        public const string Meta = "row.meta";
        public const string Summary = "row.summary";
        public const string Data = "row.data";
    }

    public static class Cell
    {
        public const string SummaryCaption = "cell.summary.caption";
        public const string TotalSummary = "cell.total.summary";
        public const string ColSummary = "cell.colsummary";
        public const string RowSummary = "cell.rowsummary";
        public const string ColMeta = "cell.col.meta";
        public const string RowMeta = "cell.row.meta";
        public const string Data = "cell.data";
    }
}

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
        sb.Append($"<tr class=\"{HtmlClasses.Row.Meta}\">");
        ProduceTableHeaderFirstCellContent();
        ProduceTableFirstRowCellSummary();
        ProduceTableHeaderRowCells(uiMatrix);
        ProduceTableLastRowCellSummary();
        sb.AppendLine("</tr>");
    }

    public void ProduceColumnSummaryRow(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var colSums = uiMatrix.ColsSummary(matrix, Options.Orientation);
        var totalSum = colSums?.Values.Sum() ?? 0;

        sb.Append($"<tr class=\"{HtmlClasses.Row.Summary}\">");
        sb.Append($"<td class=\"{HtmlClasses.Cell.SummaryCaption}\"><b>Σ</b></td>");

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append($"<td class =\"{HtmlClasses.Cell.TotalSummary}\"><a href=\"index.html\">{totalSum}</a></td>");

        ProduceColumnSummaryCells(uiMatrix, colSums);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append($"<td class =\"{HtmlClasses.Cell.TotalSummary}\"><a href=\"index.html\">{totalSum}</a></td>");

        sb.AppendLine("</tr>");
    }

    public void ProduceDataRow(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        sb.Append($"<tr class=\"{HtmlClasses.Row.Data}\">");
        ProduceRowHeaderCell(row);

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            ProduceRowSummaryCell(row, uiMatrix, matrix);

        ProduceDataCells(row, uiMatrix, matrix);

        ProduceRowSummaryCellLast(row, uiMatrix, matrix);

        sb.AppendLine("</tr>");
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

    private void ProduceTableHeaderFirstCellContent() => sb.AppendLine($"<th><a href=\"\">{(Options.Orientation == MatrixOrientation.ActionRows ? "Action \\ Domain" : "Domain \\ Action")}</a></th>");

    private void ProduceTableFirstRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append($"<th class=\"{HtmlClasses.Cell.SummaryCaption}\"><b>Σ</b></th>");
    }

    private void ProduceTableLastRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append($"<th class=\"{HtmlClasses.Cell.SummaryCaption}\"><b>Σ</b></th>");
    }

    private void ProduceTableHeaderRowCells(UiMatrix uiMatrix)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var hRef = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<th class=\"{HtmlClasses.Cell.ColMeta}\"><a href=\"{hRef}\">{metaName}</a></th>");
        }
    }

    private void ProduceRowHeaderCell(string metaName)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{metaName}.html"
            : $"domain_{metaName}.html";

        sb.Append($"<td class=\"{HtmlClasses.Cell.RowMeta}\"><a href=\"{hRef}\">{metaName}</a></td>");
    }

    private void ProduceColumnSummaryCells(UiMatrix uiMatrix, Dictionary<string, int>? colSums)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var sum = colSums?[metaName];
            var href = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<td class=\"{HtmlClasses.Cell.ColSummary}\"><a href=\"{href}\">{sum}</a></td>");
        }
    }

    private void ProduceRowSummaryCell(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        sb.Append($"<td class=\"{HtmlClasses.Cell.RowSummary}\"><a href=\"{hRef}\">{rowSum}</a></td>");
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
            var bgColor = HtmlProducer.GetCoverageColorForCell(cell, hasMethods ? methods : null, contextLookup, GetCoverageValue);
            if(bgColor != null)
            {
                style = $" style=\"inherited;background-color:{bgColor}; color:black\"";
            }

            sb.Append($"<td class=\"{HtmlClasses.Cell.Data}\"{style}><a href=\"{hRef}\">{(hasMethods ? methods?.Count ?? 0 : "&nbsp;")}</a></td>");
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
        sb.Append($"<td class=\"{HtmlClasses.Cell.RowSummary}\"><a href=\"{rowFile}\">{rowSum}</a></td>");
    }

    private static int GetCoverageValue(ContextInfo? ctx)
    {
        return ctx?.GetDimensionIntValue("coverage") ?? 0;
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
}

internal class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}
