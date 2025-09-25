using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

public class CSharpSignatureBuilder
{
    private bool _includeGenerics = true;
    private bool _includeParameters = true;
    private bool _includeReturnType = true;
    private bool _includeNamespace = false;
    private bool _includeContainingType = true;

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
            IMethodSymbol methodSymbol => BuildMethodSignature(methodSymbol),
            ITypeSymbol typeSymbol => BuildTypeSignature(typeSymbol),
            _ => BuildDefault(symbol)
        };
    }

    private string BuildTypeSignature(ITypeSymbol typeSymbol)
    {
        var (name, generics) = typeSymbol.GetTypeDetails();
        var sb = new StringBuilder();

        if (_includeNamespace && !typeSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(typeSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (_includeContainingType && typeSymbol.ContainingType != null)
        {
            sb.Append(typeSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(name);

        if (_includeGenerics && generics.Any())
        {
            sb.Append($"<{string.Join(", ", generics)}>");
        }

        return sb.ToString();
    }

    private string BuildMethodSignature(IMethodSymbol methodSymbol)
    {
        var (ReturnType, Name, GenericParameters, Parameters) = methodSymbol.GetMethodDetails();
        var sb = new StringBuilder();

        if (_includeReturnType)
        {
            sb.Append(ReturnType).Append(' ');
        }

        if (_includeNamespace && !methodSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.Append(methodSymbol.ContainingNamespace.ToDisplayString()).Append('.');
        }

        if (_includeContainingType && methodSymbol.ContainingType != null)
        {
            sb.Append(methodSymbol.ContainingType.Name).Append('.');
        }

        sb.Append(Name);

        if (_includeGenerics && GenericParameters.Any())
        {
            sb.Append($"<{string.Join(", ", GenericParameters)}>");
        }

        if (_includeParameters)
        {
            sb.Append('(');
            sb.Append(string.Join(", ",
                Parameters.Select(p => $"{p.Type} {p.Name}")));
            sb.Append(')');
        }

        return sb.ToString();
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
