using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.Invocations;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

public class RoslynInvocationExpressionWrapper : IInvocationNodeWrapper
{
    private readonly InvocationExpressionSyntax _invocation;
    private readonly ISemanticInvocationResolver _semanticInvocationResolver;

    public object Expression => _invocation.Expression;

    public RoslynInvocationExpressionWrapper(InvocationExpressionSyntax invocation, ISemanticInvocationResolver semanticInvocationResolver)
    {
        _invocation = invocation;
        _semanticInvocationResolver = semanticInvocationResolver;
    }

    public ISemanticModelWrapper? GetSemanticModel()
    {
        var syntaxTreeWrapper = new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
        if (syntaxTreeWrapper == null)
        {
            // _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{_invocation}]");
            return null;
        }

        return _semanticInvocationResolver.Resolve(syntaxTreeWrapper);
    }

    public ISyntaxTreeWrapper GetTree()
    {
        return new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
    }
}