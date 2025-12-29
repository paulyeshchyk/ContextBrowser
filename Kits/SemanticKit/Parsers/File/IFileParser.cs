using System.Collections.Generic;
using System.Threading;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

public interface IFileParser
{
    IEnumerable<ContextInfo> ParseFiles(string[] filePaths, SemanticOptions options, CancellationToken ct);

    void RenewContextInfoList(IEnumerable<ContextInfo> contextInfoList);
}
