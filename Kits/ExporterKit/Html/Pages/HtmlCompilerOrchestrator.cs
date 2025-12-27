using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;

namespace ExporterKit.Html.Pages;

public interface IHtmlCompilerOrchestrator
{
    Task CompileAllAsync(CancellationToken cancellationToken);
}

public class HtmlCompilerOrchestrator : IHtmlCompilerOrchestrator
{
    private readonly IEnumerable<IHtmlPageCompiler> _compilers;

    public HtmlCompilerOrchestrator(IEnumerable<IHtmlPageCompiler> compilers, IAppOptionsStore optionsStore)
    {
        _compilers = compilers;
    }

    public async Task CompileAllAsync(CancellationToken cancellationToken)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.CompileAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}