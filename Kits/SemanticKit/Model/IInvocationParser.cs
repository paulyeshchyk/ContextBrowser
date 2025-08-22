using ContextKit.Model;

namespace SemanticKit.Model;

public interface IInvocationParser
{
    void ParseCode(string code, string filePath, CancellationToken cancellationToken);

    void ParseFile(string filePath, CancellationToken cancellationToken);

    IEnumerable<ContextInfo> ParseFiles(string[] filePaths, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList);
}
