﻿using ContextBrowser.Generator.Matrix;
using ContextBrowser.Parser;

namespace ContextBrowser.Generator.Html;

public static class IndexGenerator
{
    public enum SummaryPlacement
    {
        None,           // не показывать
        AfterFirst,     // сразу после первой строки / колонки
        AfterLast       // внизу таблицы / справа от последней колонки
    }

    //context: build, html, page, index, file, matrix
    public static void GenerateContextIndexHtml(Dictionary<ContextContainer, List<string>> matrix, string outputFile, UnclassifiedPriority priority = UnclassifiedPriority.None, MatrixOrientation orientation = MatrixOrientation.DomainRows, SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst)
    {
        var uiMatrix = UiMatrixGenerator.Generate(matrix, orientation, priority);

        var producer = new HtmlProducer(new HtmlTableOptions { SummaryPlacement = summaryPlacement, Orientation = orientation });

        producer.ProduceHtmlStart();
        producer.ProduceHead();
        producer.ProduceHtmlBodyStart();
        producer.ProduceTitle();
        producer.ProduceTableStart();
        producer.ProduceMatrix(uiMatrix, matrix);
        producer.ProduceTableEnd();
        producer.ProduceHtmlBodyEnd();
        producer.ProduceHtmlEnd();
        var result = producer.GetResult();

        File.WriteAllText(outputFile, result);
    }
}
