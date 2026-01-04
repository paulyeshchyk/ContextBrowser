using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISyntaxTreeParser<TSyntaxTreeWrapper>
where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<IEnumerable<TSyntaxTreeWrapper>> ParseFilesToSyntaxTreesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken);
}
