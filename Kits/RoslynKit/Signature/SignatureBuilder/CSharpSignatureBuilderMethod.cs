using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

// context: roslyn, signature, build
internal class CSharpSignatureBuilderMethod : SignatureBuilder
{
    public CSharpSignatureBuilderMethod(SignatureBuilder sb) : base(sb)
    {

    }

    // context: roslyn, signature, build
    public override string Build(ISymbol symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            throw new Exception("IMethodSymbol is expected");

        var methodDetailDto = methodSymbol.GetMethodDetails();
        var sb = new StringBuilder();

        if (_includeReturnType)
        {
            sb.Append(methodDetailDto.ReturnType).Append(' ');
        }

        if (_includeNamespace && !methodSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(methodSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (_includeContainingType && methodSymbol.ContainingType != null)
        {
            sb.Append(methodSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(methodDetailDto.Name);

        if (_includeGenerics && methodDetailDto.GenericParameters.Any())
        {
            sb.Append($"<{string.Join(", ", methodDetailDto.GenericParameters)}>");
        }

        if (_includeParameters)
        {
            sb.Append('(');
            sb.Append(string.Join(", ", methodDetailDto.Parameters.Select(p => $"{p.Type} {p.Name}")));
            sb.Append(')');
        }

        return sb.ToString();
    }
}
