using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface IInvocationParser
{
    void ParseCode(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    void ParseFile(string filePath, SemanticOptions options, CancellationToken cancellationToken);

    IEnumerable<ContextInfo> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken cancellationToken);

    void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList);
}
