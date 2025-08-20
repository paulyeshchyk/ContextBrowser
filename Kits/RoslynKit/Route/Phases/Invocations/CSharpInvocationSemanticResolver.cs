using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Route.Wrappers.Meta;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Invocations;

//context: roslyn, read
public class CSharpInvocationSemanticResolver : ISemanticInvocationResolver
{
    private readonly ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> _storage;

    //context: roslyn, read
    public CSharpInvocationSemanticResolver(ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper> storage)
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
    public ISemanticModelWrapper? Resolve(InvocationExpressionSyntax expressionSyntax)
    {
        var syntaxTree = expressionSyntax.SyntaxTree;
        var treeWrapper = new RoslynSyntaxTreeWrapper(syntaxTree);
        return Resolve(treeWrapper);
    }
}