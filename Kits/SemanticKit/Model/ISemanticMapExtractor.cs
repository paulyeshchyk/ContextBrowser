using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticMapExtractor<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<SemanticCompilationMap<TSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken);
    Task<SemanticCompilationMap<TSyntaxTreeWrapper>> CreateSemanticMapFromFilesAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken);
}
