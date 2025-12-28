using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderType : SignatureBuilder
{
    public CSharpSignatureBuilderType(SignatureBuilder sb) : base(sb)
    {

    }

    // context: roslyn, signature, build
    public override string Build(ISymbol symbol)
    {
        if (symbol is not ITypeSymbol typeSymbol)
            throw new Exception("ITypeSymbol is expected");

        var typeDetailDto = typeSymbol.GetTypeDetails();
        var sb = new StringBuilder();

        if (_includeNamespace && !typeSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(typeSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (_includeContainingType && typeSymbol.ContainingType != null)
        {
            sb.Append(typeSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(typeDetailDto.Name);

        if (_includeGenerics && typeDetailDto.GenericParameters.Any())
        {
            sb.Append($"<{string.Join(", ", typeDetailDto.GenericParameters)}>");
        }

        return sb.ToString();
    }
}