using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Route.Phases.Invocations;
using SemanticKit.Model;

namespace RoslynKit.Route.Wrappers.Meta;

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

    /// <summary>
    /// ������� � ���������� ������������� ������ ��� ������� ������.
    /// </summary>
    public ISemanticModelWrapper? GetSemanticModel()
    {
        var syntaxTreeWrapper = new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
        if(syntaxTreeWrapper == null)
        {
            // ����� ����� �������� �����������, ���� ����������
            // _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{_invocation}]");
            return null;
        }

        var semanticModel = _semanticInvocationResolver.Resolve(syntaxTreeWrapper);

        // ���� Roslyn-������ �������, ����������� �� � ��� ������������� ���������
        return semanticModel;
    }

    public ISyntaxTreeWrapper GetTree()
    {
        return new RoslynSyntaxTreeWrapper(_invocation.SyntaxTree);
    }
}