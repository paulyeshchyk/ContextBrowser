using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Converters;

public static class CSharpInvocationSyntaxWrapperConverter
{
    public static CSharpInvocationSyntaxWrapper FromSymbols(ISymbol symbol, ExpressionSyntax syntax)
    {
        var wrapper = new CSharpInvocationSyntaxWrapper();

        wrapper.ShortName = symbol.GetShortName();
        wrapper.FullName = symbol.GetFullMemberName(includeParams: true);
        wrapper.Name = symbol.GetNameAndClassOwnerName();
        wrapper.Namespace = symbol.GetNamespaceOrGlobal();
        wrapper.Identifier = symbol.GetFullMemberName(includeParams: true);

        wrapper.SpanStart = syntax.Span.Start;
        wrapper.SpanEnd = syntax.Span.End;
        wrapper.IsValid = symbol is IMethodSymbol;

        return wrapper;
    }

    public static CSharpInvocationSyntaxWrapper FromSignature(
        CSharpMethodSignature signature,
        bool isPartial,
        int spanStart,
        int spanEnd)
    {
        var wrapper = new CSharpInvocationSyntaxWrapper();

        wrapper.IsPartial = isPartial;
        wrapper.FullName = signature.GetFullName();
        wrapper.Identifier = signature.GetIdentifier();
        wrapper.ShortName = signature.GetShortName();
        wrapper.Name = signature.GetMethodName();
        wrapper.Namespace = signature.GetNamespace();
        wrapper.SpanStart = spanStart;
        wrapper.SpanEnd = spanEnd;
        wrapper.IsValid = true;
        wrapper.Signature = new CSharpMethodSignature
        (
            ResultType: signature.ResultType,
            Namespace: signature.Namespace,
            ClassName: signature.ClassName,
            MethodName: signature.MethodName,
            Arguments: signature.Arguments,
            Raw: signature.Raw);

        return wrapper;
    }

    public static ISyntaxWrapper? FromExpression(
        ExpressionSyntax byInvocation,
        SemanticOptions _options)
    {
        var (raw, isPartial) = CSharpExpressionSyntaxExtensionConverter.ConvertToMethodRawSignature(byInvocation, _options);
        if (string.IsNullOrEmpty(raw))
            return default;

        var signature = SignatureUtils.Parse(raw);

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