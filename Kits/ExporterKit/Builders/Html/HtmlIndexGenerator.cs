using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Matrix;
using ExporterKit.Matrix;
using HtmlKit.Document;

namespace ExporterKit.HtmlPageSamples;

//context: build, html
public static partial class HtmlIndexGenerator
{
    //context: build, html
    public static void GenerateContextIndexHtml(IContextClassifier contextClassifier, IContextInfoData matrix, Dictionary<string, ContextInfo> allContextInfo, string outputFile, ExportMatrixOptions matrixOptions)
    {
        var uiMatrix = HtmlMatrixGenerator.Generate(contextClassifier, matrix, matrixOptions.HtmlTable.Orientation, matrixOptions.UnclassifiedPriority);
        var producer = new HtmlPageMatrix(uiMatrix, matrix, matrixOptions.HtmlTable, allContextInfo);
        producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();

        File.WriteAllText(outputFile, result);
    }
}