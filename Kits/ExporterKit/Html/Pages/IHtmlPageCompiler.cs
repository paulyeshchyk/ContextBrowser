using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace HtmlKit.Page.Compiler;

public interface IHtmlPageCompiler
{
    void Compile(IContextInfoDataset<ContextInfo> contextInfoDataset, IContextClassifier contextClassifier, ExportOptions exportOptions);
}
