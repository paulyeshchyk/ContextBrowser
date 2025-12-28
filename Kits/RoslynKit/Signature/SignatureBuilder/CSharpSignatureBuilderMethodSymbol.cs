using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using RoslynKit.AWrappers;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderMethodSymbol : SignatureBuilder
{
    public CSharpSignatureBuilderMethodSymbol(SignatureBuilder sb) : base(sb)
    {

    }

    // context: roslyn, signature, build
    public override string Build(ISymbol symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            throw new Exception("IMethodSymbol is expected");

        var methodDetailDto = methodSymbol.GetMethodDetails();
        var sb = new StringBuilder();

        if (IncludeReturnType)
        {
            sb.Append(methodDetailDto.ReturnType).Append(' ');
        }

        if (IncludeNamespace && !methodSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(methodSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (IncludeContainingType && methodSymbol.ContainingType != null)
        {
            sb.Append(methodSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(methodDetailDto.Name);

        if (IncludeGenerics && methodDetailDto.GenericParameters.Any())
        {
            sb.Append($"<{string.Join(", ", methodDetailDto.GenericParameters)}>");
        }

        if (IncludeParameters)
        {
            sb.Append('(');
            sb.Append(string.Join(", ", methodDetailDto.Parameters.Select(p => $"{p.Type} {p.Name}")));
            sb.Append(')');
        }

        return sb.ToString();
    }
}
