using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Semantic;

//context: csharp, read
public class RoslynCodeSemanticInvocationResolver : ISemanticInvocationResolver
{
    private readonly ISemanticModelStorage _storage;

    //context: csharp, read
    public RoslynCodeSemanticInvocationResolver(ISemanticModelStorage storage)
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