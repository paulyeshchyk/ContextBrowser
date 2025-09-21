using System.IO;
using HtmlKit.Matrix;
using HtmlKit.Options;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlTensorWriter<TTensor>
    where TTensor : ITensor
{
    void Write(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options);
}
