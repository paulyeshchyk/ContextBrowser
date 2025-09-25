using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

public abstract class SignatureBuilder
{
    public bool _includeGenerics = true;
    public bool _includeParameters = true;
    public bool _includeReturnType = true;
    public bool _includeNamespace = false;
    public bool _includeContainingType = true;
}

public class CSharpSignatureBuilder: SignatureBuilder
{

    public CSharpSignatureBuilder IncludeGenerics(bool value)
    {
        _includeGenerics = value;
        return this;
    }

    public CSharpSignatureBuilder IncludeParameters(bool value)
    {
        _includeParameters = value;
        return this;
    }

    public CSharpSignatureBuilder IncludeReturnType(bool value)
    {
        _includeReturnType = value;
        return this;
    }

    public CSharpSignatureBuilder IncludeNamespace(bool value)
    {
        _includeNamespace = value;
        return this;
    }

    public CSharpSignatureBuilder IncludeContainingType(bool value)
    {
        _includeContainingType = value;
        return this;
    }

    public string Build(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol methodSymbol => new CSharpMethodSignatureBuilder().BuildMethodSignature(methodSymbol),
            ITypeSymbol typeSymbol => new CSharpTypeSignatureBuilder().BuildTypeSignature(typeSymbol),
            _ => BuildDefault(symbol)
        };
    }

    private string BuildDefault(ISymbol symbol)
    {
        if (_includeNamespace && symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace)
        {
            return $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
        }
        return symbol.Name;
    }
}

internal class CSharpTypeSignatureBuilder : SignatureBuilder
{
    public string BuildTypeSignature(ITypeSymbol typeSymbol)
    {
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

internal class CSharpMethodSignatureBuilder: SignatureBuilder
{

    public string BuildMethodSignature(IMethodSymbol methodSymbol)
    {
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
