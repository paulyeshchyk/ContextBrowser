using System.IO;
using ContextBrowserKit.Options;
using HtmlKit.Matrix;

namespace HtmlKit.Document;

public interface IHtmlTensorWriter<TTensor>
    where TTensor : notnull
{
    void Write(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options);
}
