using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

// context: roslyn, signature, build
public static class CSharpSignatureBuilderFactoryExtensions
{
    // context: roslyn, signature, build
    public static string BuildShortName(this ISymbol symbol)
    {
        var builder = SignatureBuilderFactory.Create(MethodRepresentationStyle.Minimal);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildNameAndClassOwnerName(this ISymbol symbol)
    {
        var builder = SignatureBuilderFactory.Create(MethodRepresentationStyle.MinimalButType);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildNameAndParams(this ISymbol symbol)
    {
        var builder = SignatureBuilderFactory.Create(MethodRepresentationStyle.Signature);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildFullMemberName(this ISymbol symbol)
    {
        var builder = SignatureBuilderFactory.Create(MethodRepresentationStyle.FullTypeArguments);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildQualifiedName(this ISymbol symbol)
    {
        var builder = SignatureBuilderFactory.Create(MethodRepresentationStyle.QualifiedSignature);
        return builder.Build(symbol);
    }
}
