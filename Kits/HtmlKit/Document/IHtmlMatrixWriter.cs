using System.IO;
using ContextBrowserKit.Matrix;
using HtmlKit.Extensions;
using HtmlKit.Options;

namespace HtmlKit.Document;

public interface IHtmlMatrixWriter
{
    void Write(TextWriter writer, IHtmlMatrix matrix, HtmlMatrixSummary? summary, HtmlTableOptions options);
}
