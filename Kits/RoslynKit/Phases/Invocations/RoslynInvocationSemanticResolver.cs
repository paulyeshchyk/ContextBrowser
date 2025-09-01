using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;

namespace RoslynKit.Phases.Invocations;

//context: roslyn, read
public class RoslynInvocationSemanticResolver : ISemanticInvocationResolver
{
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _storage;

    //context: roslyn, read
    public RoslynInvocationSemanticResolver(ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> storage)
    {
        _storage = storage;
    }

    //context: roslyn, read
    public ISemanticModelWrapper? Resolve(ISyntaxTreeWrapper? syntaxTree)
    {
        if (syntaxTree == null)
            return null;

        return _storage.GetModel(syntaxTree);
    }

    //context: roslyn, read
    public ISemanticModelWrapper? Resolve(object expressionSyntax)
    {
        if (expressionSyntax is not InvocationExpressionSyntax invocationExpressionSyntax)
        {
            throw new Exception($"expressionSyntax is not InvocationExpressionSyntax ({expressionSyntax})");
        }

        var syntaxTree = invocationExpressionSyntax.SyntaxTree;
        var treeWrapper = new RoslynSyntaxTreeWrapper(syntaxTree);
        return Resolve(treeWrapper);
    }
}