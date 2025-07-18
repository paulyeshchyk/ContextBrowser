using CB.Generator.Matrix;
using ContextBrowser.Parser;
using System.Text;

namespace CB.Generator.Html;

public static class IndexGenerator
{
    public enum SummaryPlacement
    {
        None,           // не показывать
        AfterFirst,     // сразу после первой строки / колонки
        AfterLast       // внизу таблицы / справа от последней колонки
    }

    //context: build, html, page, index, file, matrix
    public static void GenerateContextIndexHtml(Dictionary<(string Action, string Domain), List<string>> matrix, string outputFile, UnclassifiedPriority priority = UnclassifiedPriority.None, MatrixOrientation orientation = MatrixOrientation.DomainRows, SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst)
    {
        var uiMatrix = UiMatrixGenerator.Generate(matrix, orientation, priority);

        var sb = new StringBuilder();
        var producer = new HtmlProducer(sb, new HtmlTableOptions { SummaryPlacement = summaryPlacement, Orientation = orientation });

        producer.ProduceHtmlStart();
        producer.ProduceHead();
        producer.ProduceHtmlBodyStart();
        producer.ProduceTitle();
        producer.ProduceTableStart();
        producer.ProduceMatrix(uiMatrix, matrix);
        producer.ProduceTableEnd();
        producer.ProduceHtmlBodyEnd();
        producer.ProduceHtmlEnd();
        File.WriteAllText(outputFile, sb.ToString());
    }

    public static void AddColSumm(StringBuilder sb, UiMatrix uiMatrix, MatrixOrientation orientation, SummaryPlacement placement, Func<string, int> colSumCallback, Func<int> totalSummCallback)
    {
        var totalSum = totalSummCallback();

        sb.Append("<tr><td><b>Σ</b></td>");

        if(placement == SummaryPlacement.AfterFirst)
        {
            // Вставляем ячейку пересечения Σ × Σ
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");
        }

        foreach(var col in uiMatrix.cols)
        {
            var sum = colSumCallback(col);
            var href = orientation == MatrixOrientation.ActionRows
                ? $"domain_{col}.html"
                : $"action_{col}.html";

            sb.Append($"<td><a href=\"{href}\">{sum}</a></td>");
        }

        if(placement == SummaryPlacement.AfterLast)
        {
            sb.Append($"<td><a href=\"index.html\">{totalSum}</a></td>");
        }

        sb.AppendLine("</tr>");
    }

    public static void AddRowSumm(StringBuilder sb, string row, MatrixOrientation orientation, Func<string, int> rowSumCallback)
    {
        var href = orientation == MatrixOrientation.ActionRows
            ? $"action_{row}.html"
            : $"domain_{row}.html";

        var sum = rowSumCallback(row);
        sb.Append($"<td><a href=\"{href}\">{sum}</a></td>");
    }
}