using Microsoft.CodeAnalysis;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderDefault : SignatureBuilder
{
    public CSharpSignatureBuilderDefault(SignatureBuilder sb) : base(sb)
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