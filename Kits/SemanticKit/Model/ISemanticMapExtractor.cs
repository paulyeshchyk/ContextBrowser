using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticMapExtractor<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<SemanticCompilationMap<TSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken);
    Task<SemanticCompilationMap<TSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(string filePath, string compilationName, CancellationToken cancellationToken);
}
