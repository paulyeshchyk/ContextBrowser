using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.Strategy.Declaration;

public interface IDeclarationParser<TContext, TSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task ParseAsync(ISemanticSyntaxRouter<TContext> router, CompilationMap<TSyntaxTreeWrapper> mapItem, CancellationToken cancellationToken);
}
