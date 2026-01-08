using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Signature.SignatureBuilder;

// context: roslyn, signature, build
public static class CSharpSignatureBuilderFactoryExtensions
{
    // context: roslyn, signature, build
    public static string BuildShortName(this ISymbol symbol)
    {
        var builder = CSharpSignatureBuilderFactory.Create(MethodRepresentationStyle.Minimal);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildNameAndClassOwnerName(this ISymbol symbol)
    {
        var builder = CSharpSignatureBuilderFactory.Create(MethodRepresentationStyle.MinimalButType);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildNameAndParams(this ISymbol symbol)
    {
        var builder = CSharpSignatureBuilderFactory.Create(MethodRepresentationStyle.Signature);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildFullMemberName(this ISymbol symbol)
    {
        var builder = CSharpSignatureBuilderFactory.Create(MethodRepresentationStyle.FullTypeArguments);
        return builder.Build(symbol);
    }

    // context: roslyn, signature, build
    public static string BuildQualifiedName(this ISymbol symbol)
    {
        var builder = CSharpSignatureBuilderFactory.Create(MethodRepresentationStyle.QualifiedSignature);
        return builder.Build(symbol);
    }
}
