using System.IO;
using ContextBrowserKit.Matrix;
using HtmlKit.Extensions;
using HtmlKit.Options;
using TensorKit.Model;

namespace HtmlKit.Document;

public interface IHtmlMatrixWriter<TTensor>
    where TTensor : ITensor<string>
{
    void Write(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options);
}
