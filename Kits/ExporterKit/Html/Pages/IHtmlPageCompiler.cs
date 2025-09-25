using System.Threading;
using System.Threading.Tasks;

namespace ExporterKit.Html.Pages;

public interface IHtmlPageCompiler
{
    Task CompileAsync(CancellationToken cancellationToken);
}
