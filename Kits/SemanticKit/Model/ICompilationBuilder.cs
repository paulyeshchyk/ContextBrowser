using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ICompilationBuilder<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<ICompilationWrapper> BuildAsync(SemanticOptions options, IEnumerable<TSyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name, CancellationToken cancellationToken);
}
