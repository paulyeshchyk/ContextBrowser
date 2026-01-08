using Microsoft.CodeAnalysis;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
public class CSharpSignatureBuilder : SignatureBuilder<ISymbol>
{
    public CSharpSignatureBuilder()
    {
    }

    public CSharpSignatureBuilder(SignatureBuilder<ISymbol> source) : base(source)
    {
    }

    public override string Build(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol methodSymbol => new CSharpSignatureBuilderMethodSymbol(this).Build(methodSymbol),
            ITypeSymbol typeSymbol => new CSharpSignatureBuilderTypeSymbol(this).Build(typeSymbol),
            _ => new CSharpSignatureBuilderDefault(this).Build(symbol)
        };
    }
}