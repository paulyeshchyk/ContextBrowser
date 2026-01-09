using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using RoslynKit.Model.Symbol;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderTypeSymbol : SignatureBuilder<ISymbol>
{
    public CSharpSignatureBuilderTypeSymbol(SignatureBuilder<ISymbol> sb) : base(sb)
    {

    }

    // context: roslyn, signature, build
    public override string Build(ISymbol symbol)
    {
        if (symbol is not ITypeSymbol typeSymbol)
            throw new Exception("ITypeSymbol is expected");

        var typeDetailDto = typeSymbol.GetTypeDetails();
        var sb = new StringBuilder();

        if (IncludeNamespace && !typeSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(typeSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (IncludeContainingType && typeSymbol.ContainingType != null)
        {
            sb.Append(typeSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(typeDetailDto.Name);

        if (IncludeGenerics && typeDetailDto.GenericParameters.Any())
        {
            sb.Append($"<{string.Join(", ", typeDetailDto.GenericParameters)}>");
        }

        return sb.ToString();
    }
}