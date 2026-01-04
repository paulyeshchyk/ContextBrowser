using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

public interface ISemanticDeclarationParser<TContext, TSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task Parse(ISemanticSyntaxRouter<TContext> router, SemanticOptions options, CompilationMap<TSyntaxTreeWrapper> mapItem, CancellationToken cancellationToken);
}
