using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace HtmlKit.Page.Compiler;

public interface IHtmlPageCompiler
{
    void Compile(IContextInfoDataset contextInfoDataset, ExportOptions exportOptions);
}
