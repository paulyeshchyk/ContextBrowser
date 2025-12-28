using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

public static class ISymbolExtensions
{
    public static string GetNamespaceOrGlobal(this ISymbol symbol)
    {
        var containingNamespace = symbol.ContainingNamespace;

        if (containingNamespace == null || containingNamespace.IsGlobalNamespace)
        {
            return "GLOBAL";
        }

        var parts = new List<string>();
        while (containingNamespace != null && !containingNamespace.IsGlobalNamespace)
        {
            parts.Add(containingNamespace.Name);
            containingNamespace = containingNamespace.ContainingNamespace;
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    public static TypeDetailDto GetTypeDetails(this ITypeSymbol typeSymbol)
    {
        var name = typeSymbol.Name;

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            var genericParameters = namedTypeSymbol.TypeArguments.Select(arg => arg.ToDisplayString()).ToList();
            return (name, genericParameters);
        }

        return (name, Enumerable.Empty<string>());
    }

    public static MethodDetailDto GetMethodDetails(this IMethodSymbol methodSymbol)
    {
        var returnType = methodSymbol.ReturnsVoid ? "void" : methodSymbol.ReturnType.ToDisplayString();
        var name = methodSymbol.Name;
        var genericParameters = methodSymbol.TypeArguments.Select(arg => arg.ToDisplayString()).ToList();
        var parameters = methodSymbol.Parameters.Select(p => (p.Type.ToDisplayString(), p.Name)).ToList();

        return (returnType, name, genericParameters, parameters);
    }
}

public record struct TypeDetailDto(string Name, IEnumerable<string> GenericParameters)
{
    public static implicit operator (string Name, IEnumerable<string> GenericParameters)(TypeDetailDto value)
    {
        return (value.Name, value.GenericParameters);
    }

    public static implicit operator TypeDetailDto((string Name, IEnumerable<string> GenericParameters) value)
    {
        return new TypeDetailDto(value.Name, value.GenericParameters);
    }
}

public record struct MethodDetailDto(string ReturnType, string Name, IEnumerable<string> GenericParameters, IEnumerable<(string Type, string Name)> Parameters)
{
    public static implicit operator (string ReturnType, string Name, IEnumerable<string> GenericParameters, IEnumerable<(string Type, string Name)> Parameters)(MethodDetailDto value)
    {
        return (value.ReturnType, value.Name, value.GenericParameters, value.Parameters);
    }

    public static implicit operator MethodDetailDto((string ReturnType, string Name, IEnumerable<string> GenericParameters, IEnumerable<(string Type, string Name)> Parameters) value)
    {
        return new MethodDetailDto(value.ReturnType, value.Name, value.GenericParameters, value.Parameters);
    }
}