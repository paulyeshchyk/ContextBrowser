using ContextKit.Model;
using ExporterKit.Matrix;
using ExporterKit.Options;
using HtmlKit.Document;

namespace ExporterKit.HtmlPageSamples;

//context: build, html
public static partial class IndexGenerator
{
    //context: build, html
    public static void GenerateContextIndexHtml(IContextClassifier contextClassifier, Dictionary<ContextContainer, List<string>> matrix, Dictionary<string, ContextInfo> allContextInfo, string outputFile, ExportMatrixOptions matrixOptions)
    {
        var uiMatrix = UiMatrixGenerator.Generate(contextClassifier, matrix, matrixOptions.HtmlTable.Orientation, matrixOptions.UnclassifiedPriority);
        var producer = new HtmlPageMatrix(uiMatrix, matrix, matrixOptions.HtmlTable, allContextInfo);
        producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();

        File.WriteAllText(outputFile, result);
    }
}