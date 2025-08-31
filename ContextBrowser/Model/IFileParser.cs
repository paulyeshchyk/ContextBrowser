using ContextKit.Model;
using SemanticKit.Model.Options;

namespace ContextBrowser.Model;

public interface IFileParser
{
    IEnumerable<ContextInfo> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList);
}
