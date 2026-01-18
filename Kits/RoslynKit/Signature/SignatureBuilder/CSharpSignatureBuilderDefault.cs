using Microsoft.CodeAnalysis;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderDefault : SignatureBuilder<ISymbol>
{
    public CSharpSignatureBuilderDefault(SignatureBuilder<ISymbol> sb) : base(sb)
    {
    }

    // context: roslyn, signature, build
    public override string Build(ISymbol symbol)
    {
        if (IncludeNamespace && symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace)
        {
            return $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
        }
        return symbol.Name;
    }
}