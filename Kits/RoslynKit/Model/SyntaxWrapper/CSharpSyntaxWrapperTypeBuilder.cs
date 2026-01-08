using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public interface ICSharpSyntaxWrapperTypeBuilder
{
    CSharpSyntaxWrapperType BuildType(ISyntaxWrapper syntaxWrapper);
}


public class CSharpSyntaxWrapperTypeBuilder : ICSharpSyntaxWrapperTypeBuilder
{

    private readonly ISignatureParser<CSharpIdentifier> _signatureParser;

    public CSharpSyntaxWrapperTypeBuilder(ISignatureParser<CSharpIdentifier> signatureParser) => _signatureParser = signatureParser;


    public CSharpSyntaxWrapperType BuildType(ISyntaxWrapper syntaxWrapper)
    {
        var result = new CSharpSyntaxWrapperType();
        result.SpanStart = syntaxWrapper.SpanStart;
        result.SpanEnd = syntaxWrapper.SpanEnd;

        if (syntaxWrapper.Signature is ISignature signature)
        {
            result.Name = signature.ClassName;
            result.ShortName = signature.ClassName;
            result.Namespace = signature.Namespace;
            result.Identifier = $"{signature.Namespace}.{signature.ClassName}";
            result.FullName = $"{signature.Namespace}.{signature.ClassName}";
            result.Signature = BuildDefault(signature);
        }
        else
        {
            var parsedSignature = _signatureParser.Parse(syntaxWrapper.FullName);

            result.FullName = $"{parsedSignature.Namespace}.{parsedSignature.ClassName}";
            result.Identifier = $"{parsedSignature.Namespace}.{parsedSignature.ClassName}";
            result.ShortName = parsedSignature.ClassName;
            result.Name = parsedSignature.ClassName;
            result.Namespace = parsedSignature.Namespace;
        }
        return result;
    }

    private static SignatureDefault BuildDefault(ISignature signature)
    {
        return new SignatureDefault
        (
            ResultType: signature.ResultType,
            Namespace: signature.Namespace,
            ClassName: signature.ClassName,
            MethodName: signature.MethodName,
            Arguments: signature.Arguments,
            Raw: signature.Raw);
    }

}