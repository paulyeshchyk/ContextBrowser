using ContextKit.Model;

namespace ContextBrowser.Model;

public interface IFileParser
{
    IEnumerable<ContextInfo> ParseFiles(string[] filePaths, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList);
}
