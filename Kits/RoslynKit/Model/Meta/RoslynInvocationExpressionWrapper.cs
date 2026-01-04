using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Meta;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

public class RoslynInvocationExpressionWrapper : IInvocationNodeWrapper<RoslynSyntaxTreeWrapper>
{
    private readonly InvocationExpressionSyntax _invocation;
    private readonly ISemanticInvocationResolver<RoslynSyntaxTreeWrapper> _semanticInvocationResolver;
    private readonly IAppLogger<AppLevel> _logger;

    public object Expression => _invocation.Expression;

    public RoslynInvocationExpressionWrapper(InvocationExpressionSyntax invocation, ISemanticInvocationResolver<RoslynSyntaxTreeWrapper> semanticInvocationResolver, IAppLogger<AppLevel> logger)
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
            return default;
        }

        return _semanticInvocationResolver.Resolve(syntaxTreeWrapper);
    }

    public RoslynSyntaxTreeWrapper BuildTree()
    {
        return new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
    }
}