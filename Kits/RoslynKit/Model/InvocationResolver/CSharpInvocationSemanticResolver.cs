using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model.Storage;

namespace RoslynKit.Model.Resolver;

//context: csharp, read
public class CSharpInvocationSemanticResolver : ISemanticInvocationResolver
{
    private readonly ISemanticModelStorage<SyntaxTree, SemanticModel> _storage;

    //context: csharp, read
    public CSharpInvocationSemanticResolver(ISemanticModelStorage<SyntaxTree, SemanticModel> storage)
    {
        _storage = storage;
    }

    //context: csharp, read
    public SemanticModel? Resolve(SyntaxTree? syntaxTree)
    {
        if(syntaxTree == null)
            return null;

        return _storage.GetModel(syntaxTree);
    }

    //context: csharp, read
    public SemanticModel? Resolve(InvocationExpressionSyntax expressionSyntax)
    {
        var syntaxTree = expressionSyntax.SyntaxTree;
        return Resolve(syntaxTree);
    }
}