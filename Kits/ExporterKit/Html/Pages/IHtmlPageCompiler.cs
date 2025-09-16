using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace HtmlKit.Page.Compiler;

public interface IHtmlPageCompiler
{
    Task CompileAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken);
}
