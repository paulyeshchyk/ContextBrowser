using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Converters;

public static class CSharpInvocationSyntaxWrapperConverter
{
    public static CSharpInvocationSyntaxWrapper FromSymbols(ISymbol symbol, ExpressionSyntax syntax)
    {
        var wrapper = new CSharpInvocationSyntaxWrapper()
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

    public static CSharpInvocationSyntaxWrapper FromSignature(
        SignatureDefault signature,
        bool isPartial,
        int spanStart,
        int spanEnd)
    {
        var wrapper = new CSharpInvocationSyntaxWrapper()
        {
            IsPartial = isPartial,
            FullName = signature.GetFullName(),
            Identifier = signature.GetIdentifier(),
            ShortName = signature.GetShortName(),
            Name = signature.GetMethodName(),
            Namespace = signature.GetNamespace(),
            SpanStart = spanStart,
            SpanEnd = spanEnd,
            IsValid = true,
            Signature = new SignatureDefault
                (ResultType: signature.ResultType,
                    Namespace: signature.Namespace,
                    ClassName: signature.ClassName,
                    MethodName: signature.MethodName,
                    Arguments: signature.Arguments,
                    Raw: signature.Raw
                )
        };
        return wrapper;
    }

    public static ISyntaxWrapper? FromExpression(
        ExpressionSyntax byInvocation,
        SemanticOptions _options)
    {
        var (raw, isPartial) = CSharpExpressionSyntaxExtensionConverter.ConvertToMethodRawSignature(byInvocation, _options);
        if (string.IsNullOrEmpty(raw))
            return default;

        var signature = CSharpSignatureUtils.Parse(raw);

        return new CSharpInvocationSyntaxWrapper()
        {
            Signature = signature,
            SpanStart = byInvocation.Span.Start,
            SpanEnd = byInvocation.Span.End,
            IsPartial = isPartial,
            FullName = signature.GetFullName(),
            ShortName = signature.GetShortName(),
            Name = signature.GetMethodName(),
            Namespace = signature.GetNamespace(),
            Identifier = signature.GetIdentifier()
        };
    }
}