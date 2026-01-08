using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Model.SyntaxWrapper;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Converters;

public interface ICSharpInvocationSyntaxWrapperConverter
{
    ISyntaxWrapper? FromExpression(ExpressionSyntax byInvocation, SemanticOptions options);
    CSharpSyntaxWrapperInvocation FromSymbols(ExpressionSyntax syntax, ISymbol symbol);
}


public class CSharpInvocationSyntaxWrapperConverter : ICSharpInvocationSyntaxWrapperConverter
{

    private readonly ISignatureParser<CSharpIdentifier> _signatureParser;

    public CSharpInvocationSyntaxWrapperConverter(ISignatureParser<CSharpIdentifier> signatureParser) => _signatureParser = signatureParser;


    public CSharpSyntaxWrapperInvocation FromSymbols(ExpressionSyntax syntax, ISymbol symbol)
    {
#warning do double check for Name = symbol.BuildNameAndClassOwnerName()
        var wrapper = new CSharpSyntaxWrapperInvocation()
        {
            ShortName = symbol.BuildShortName(),
            FullName = symbol.BuildFullMemberName(),
            Name = symbol.BuildShortName(),
            Namespace = symbol.GetNamespaceOrGlobal(),
            Identifier = symbol.BuildFullMemberName(),

            SpanStart = syntax.Span.Start,
            SpanEnd = syntax.Span.End,
            IsValid = symbol is IMethodSymbol
        };
        return wrapper;
    }

    public ISyntaxWrapper? FromExpression(ExpressionSyntax byInvocation, SemanticOptions options)
    {
        var raw = byInvocation.ConvertToMethodRawSignature(options);
        if (string.IsNullOrEmpty(raw))
            return null;

        var signature = _signatureParser.Parse(raw);

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