using ContextBrowser.Generator.Matrix;
using ContextBrowser.Parser;
using System.Text;
using static ContextBrowser.Generator.Html.IndexGenerator;

namespace ContextBrowser.Generator.Html;

internal class HtmlProducer
{
    private readonly StringBuilder sb = new StringBuilder();
    public readonly HtmlTableOptions Options;

    public HtmlProducer(HtmlTableOptions options)
    {
        Options = options;
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

    private void ProduceTableHeaderFirstCellContent() =>
        sb.AppendLine($"<th><a href=\"\">{(Options.Orientation == MatrixOrientation.ActionRows ? "Action \\ Domain" : "Domain \\ Action")}</a></th>");

    private void ProduceTableFirstRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append("<th><b>Σ</b></th>");
    }

    private void ProduceTableLastRowCellSummary()
    {
        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append("<th><b>Σ</b></th>");
    }

    private void ProduceTableHeaderRowCells(UiMatrix uiMatrix)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var hRef = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<th><a href=\"{hRef}\">{metaName}</a></th>");
        }
    }

    private void ProduceRowHeaderCell(string metaName)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{metaName}.html"
            : $"domain_{metaName}.html";

        sb.Append($"<td><a href=\"{hRef}\">{metaName}</a></td>");
    }

    private void ProduceColumnSummaryCells(UiMatrix uiMatrix, Dictionary<string, int>? colSums)
    {
        foreach(var metaName in uiMatrix.cols)
        {
            var sum = colSums?[metaName];
            var href = Options.Orientation == MatrixOrientation.ActionRows
                ? $"domain_{metaName}.html"
                : $"action_{metaName}.html";

            sb.Append($"<td><a href=\"{href}\">{sum}</a></td>");
        }
    }

    private void ProduceRowSummaryCell(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        var hRef = Options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var rowSum = uiMatrix.RowsSummary(matrix, Options.Orientation)?[row];
        sb.Append($"<td><a href=\"{hRef}\">{rowSum}</a></td>");
    }

    public void ProduceTableHeaderRow(UiMatrix uiMatrix)
    {
        sb.Append("<tr>");
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

        sb.Append("<tr>");
        sb.Append("<td><b>Σ</b></td>");

        if(Options.SummaryPlacement == SummaryPlacement.AfterFirst)
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");

        ProduceColumnSummaryCells(uiMatrix, colSums);

        if(Options.SummaryPlacement == SummaryPlacement.AfterLast)
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");

        sb.AppendLine("</tr>");
    }


    private void ProduceDataCells(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        foreach(var col in uiMatrix.cols)
        {
            var key = Options.Orientation == MatrixOrientation.ActionRows
                ? (row, col)
                : (col, row);

            if(matrix.TryGetValue(key, out var methods) && methods.Any())
            {
                var hRef = $"composite_{key.Item1}_{key.Item2}.html";
                sb.Append($"<td><a href=\"{hRef}\">{methods.Count}</a></td>");
            }
            else
            {
                sb.Append("<td>&nbsp;</td>");
            }
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
        sb.Append($"<td><a href=\"{rowFile}\">{rowSum}</a></td>");
    }

    public void ProduceDataRow(string row, UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix)
    {
        sb.Append("<tr>");
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
}


internal class HtmlTableOptions
{
    public SummaryPlacement SummaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation Orientation = MatrixOrientation.DomainRows;
}