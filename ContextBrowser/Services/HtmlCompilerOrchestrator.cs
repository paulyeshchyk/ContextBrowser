using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using HtmlKit.Page.Compiler;

namespace ContextBrowser.Services;

public interface IHtmlCompilerOrchestrator
{
    void CompileAll(IContextInfoDataset dataset, ExportOptions exportOptions);
}

public class HtmlCompilerOrchestrator : IHtmlCompilerOrchestrator
{
    private readonly IEnumerable<IHtmlPageCompiler> _compilers;

    public HtmlCompilerOrchestrator(IEnumerable<IHtmlPageCompiler> compilers)
    {
        _compilers = compilers;
    }

    public void CompileAll(IContextInfoDataset dataset, ExportOptions exportOptions)
    {
        foreach (var compiler in _compilers)
        {
            compiler.Compile(dataset, exportOptions);
        }
    }
}