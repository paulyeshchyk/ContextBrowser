using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Page.Compiler;

namespace ContextBrowser.Services;

public interface IHtmlCompilerOrchestrator
{
    Task CompileAllAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken);
}

public class HtmlCompilerOrchestrator : IHtmlCompilerOrchestrator
{
    private readonly IEnumerable<IHtmlPageCompiler> _compilers;

    public HtmlCompilerOrchestrator(IEnumerable<IHtmlPageCompiler> compilers)
    {
        _compilers = compilers;
    }

    public async Task CompileAllAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.CompileAsync(contextClassifier, exportOptions, cancellationToken);
        }
    }
}