using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

public class RoslynInvocationExpressionWrapper : IInvocationNodeWrapper
{
    private readonly InvocationExpressionSyntax _invocation;
    private readonly ISemanticInvocationResolver _semanticInvocationResolver;
    private readonly IAppLogger<AppLevel> _logger;

    public object Expression => _invocation.Expression;

    public RoslynInvocationExpressionWrapper(InvocationExpressionSyntax invocation, ISemanticInvocationResolver semanticInvocationResolver, IAppLogger<AppLevel> logger)
    {
        _invocation = invocation;
        _semanticInvocationResolver = semanticInvocationResolver;
        _logger = logger;
    }

    public ISemanticModelWrapper? GetSemanticModel()
    {
        var syntaxTreeWrapper = new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
        if (syntaxTreeWrapper == null)
        {
            _logger.WriteLog(AppLevel.R_Invocation, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{_invocation}]");
            return null;
        }

        return _semanticInvocationResolver.Resolve(syntaxTreeWrapper);
    }

    public ISyntaxTreeWrapper BuildTree()
    {
        return new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
    }
}