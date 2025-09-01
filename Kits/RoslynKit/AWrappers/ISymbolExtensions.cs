using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Extensions;

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

    public static string GetShortName(this ISymbol symbol)
    {
        // старое поведение: только имя без параметров, без возврата
        var builder = new CSharpSignatureBuilder()
            .IncludeGenerics(false)
            .IncludeParameters(false)
            .IncludeReturnType(false)
            .IncludeNamespace(false)
            .IncludeContainingType(false);

        return builder.Build(symbol);
    }

    public static string GetNameAndClassOwnerName(this ISymbol symbol)
    {
        var builder = new CSharpSignatureBuilder()
            .IncludeGenerics(false)
            .IncludeParameters(false)
            .IncludeReturnType(false)
            .IncludeNamespace(false)
            .IncludeContainingType(true);

        return builder.Build(symbol);
    }

    public static string GetNameAndParams(this ISymbol symbol)
    {
        var builder = new CSharpSignatureBuilder()
            .IncludeGenerics(false)
            .IncludeParameters(true)
            .IncludeReturnType(true)
            .IncludeNamespace(false)
            .IncludeContainingType(false);

        return builder.Build(symbol);
    }

    public static string GetFullMemberName(this ISymbol symbol, bool includeParams)
    {
        // имя метода с с нэймспейс с типом + параметры (опционально), возврат включён
        var builder = new CSharpSignatureBuilder()
            .IncludeGenerics(true)
            .IncludeParameters(includeParams)
            .IncludeReturnType(true)
            .IncludeNamespace(true)
            .IncludeContainingType(true);

        return builder.Build(symbol);
    }

    public static string GetQualifiedName(this ISymbol symbol)
    {
        // максимально подробное имя без namespace, с return type, с params, с generics
        var builder = new CSharpSignatureBuilder()
            .IncludeGenerics(true)
            .IncludeParameters(true)
            .IncludeReturnType(true)
            .IncludeNamespace(false)
            .IncludeContainingType(false);

        return builder.Build(symbol);
    }

    public static (string Name, IEnumerable<string> GenericParameters) GetTypeDetails(this ITypeSymbol typeSymbol)
    {
        var name = typeSymbol.Name;

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            var genericParameters = namedTypeSymbol.TypeArguments.Select(arg => arg.ToDisplayString()).ToList();
            return (name, genericParameters);
        }

        return (name, Enumerable.Empty<string>());
    }

    public static (string ReturnType, string Name, IEnumerable<string> GenericParameters, IEnumerable<(string Type, string Name)> Parameters)
        GetMethodDetails(this IMethodSymbol methodSymbol)
    {
        var returnType = methodSymbol.ReturnsVoid ? "void" : methodSymbol.ReturnType.ToDisplayString();
        var name = methodSymbol.Name;
        var genericParameters = methodSymbol.TypeArguments.Select(arg => arg.ToDisplayString()).ToList();
        var parameters = methodSymbol.Parameters.Select(p => (p.Type.ToDisplayString(), p.Name)).ToList();

        return (returnType, name, genericParameters, parameters);
    }
}
