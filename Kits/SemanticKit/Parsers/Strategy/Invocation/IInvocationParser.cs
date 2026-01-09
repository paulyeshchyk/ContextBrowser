using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using SemanticKit.Model;

namespace SemanticKit.Parsers.Strategy.Invocation;

public interface IInvocationParser<TContext, TSyntaxTreeWrapper>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    Task<bool> ParseInvocationsAsync(string code, string filePath, CancellationToken cancellationToken);
}
