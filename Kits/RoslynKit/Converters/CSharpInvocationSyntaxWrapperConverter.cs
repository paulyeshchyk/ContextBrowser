using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Converters;

public static class CSharpInvocationSyntaxWrapperConverter
{
    public static CSharpSyntaxWrapperInvocation FromSymbols(ISymbol symbol, ExpressionSyntax syntax)
    {
        var wrapper = new CSharpSyntaxWrapperInvocation()
        {
            ShortName = symbol.BuildShortName(),
            FullName = symbol.BuildFullMemberName(),
            Name = symbol.BuildNameAndClassOwnerName(),
            Namespace = symbol.GetNamespaceOrGlobal(),
            Identifier = symbol.BuildFullMemberName(),

            SpanStart = syntax.Span.Start,
            SpanEnd = syntax.Span.End,
            IsValid = symbol is IMethodSymbol
        };
        return wrapper;
    }

    public static ISyntaxWrapper? FromExpression(
        ExpressionSyntax byInvocation,
        SemanticOptions options)
    {
        var raw = byInvocation.ConvertToMethodRawSignature(options);
        if (string.IsNullOrEmpty(raw))
            return null;

        var signature = CSharpSignatureUtils.Parse(raw);

        return new CSharpSyntaxWrapperInvocation()
        {
            Signature = signature,
            SpanStart = byInvocation.Span.Start,
            SpanEnd = byInvocation.Span.End,
            FullName = signature.GetFullName(),
            ShortName = signature.GetShortName(),
            Name = signature.GetMethodName(),
            Namespace = signature.GetNamespace(),
            Identifier = signature.GetIdentifier()
        };
    }
}