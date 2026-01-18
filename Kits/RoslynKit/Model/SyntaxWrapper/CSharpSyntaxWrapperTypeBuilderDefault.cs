using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public interface ICSharpSyntaxWrapperTypeBuilderDefault
{
    CSharpSyntaxWrapperType BuildDefaultWrapper(ISyntaxWrapper syntaxWrapper);
}

public interface ICSharpSyntaxWrapperTypeBuilderSigned
{
    CSharpSyntaxWrapperType BuildSignedWrapper(ISyntaxWrapper syntaxWrapper, ISignature signature);
}

public class CSharpSyntaxWrapperTypeBuilderSigned : ICSharpSyntaxWrapperTypeBuilderSigned
{
    private readonly ISignatureParser<CSharpIdentifier> _signatureParser;

    public CSharpSyntaxWrapperTypeBuilderSigned(ISignatureParser<CSharpIdentifier> signatureParser) => _signatureParser = signatureParser;

    public CSharpSyntaxWrapperType BuildSignedWrapper(ISyntaxWrapper syntaxWrapper, ISignature signature)
    {
        var args = BuildArgumentsFromExistingSignature(syntaxWrapper, signature);

        return new CSharpSyntaxWrapperType(
            name: args.Name,
            @namespace: args.Namespace,
            spanEnd: args.SpanEnd,
            spanStart: args.SpanStart,
            fullName: args.FullName,
            identifier: args.Identifier,
            shortName: args.ShortName,
            isValid: args.IsValid,
            signature: args.Signature
        );
    }

    /// <summary>
    /// Собирает аргументы для конструктора, используя уже доступную ISignature.
    /// </summary>
    private CSharpSyntaxWrapperTypeArguments BuildArgumentsFromExistingSignature(ISyntaxWrapper syntaxWrapper, ISignature signature)
    {
        var fullName = $"{signature.Namespace}.{signature.ClassName}";

        return new CSharpSyntaxWrapperTypeArguments(
            Name: signature.ClassName,
            Namespace: signature.Namespace,
            SpanEnd: syntaxWrapper.SpanEnd,
            SpanStart: syntaxWrapper.SpanStart,
            FullName: fullName,
            Identifier: fullName, // Identifier совпадает с FullName
            ShortName: signature.ClassName,
            IsValid: true,
            Signature: BuildDefault(signature)
        );
    }

    /// <summary>
    /// Собирает аргументы для конструктора, выполняя парсинг.
    /// </summary>
    private CSharpSyntaxWrapperTypeArguments BuildArgumentsFromParsing(ISyntaxWrapper syntaxWrapper)
    {
        var parsedSignature = _signatureParser.Parse(syntaxWrapper.FullName);

        // TODO: подмена namespace
        // Логика подмены перенесена сюда, как и планировалось в исходном коде
        var finalNamespace = syntaxWrapper.Namespace.Equals(parsedSignature.Namespace, StringComparison.InvariantCulture)
            ? parsedSignature.Namespace
            : syntaxWrapper.Namespace;

        var fullName = $"{finalNamespace}.{parsedSignature.ClassName}";

        return new CSharpSyntaxWrapperTypeArguments(
            Name: parsedSignature.ClassName,
            Namespace: finalNamespace,
            SpanEnd: syntaxWrapper.SpanEnd,
            SpanStart: syntaxWrapper.SpanStart,
            FullName: fullName,
            Identifier: fullName, // Identifier совпадает с FullName
            ShortName: parsedSignature.ClassName,
            IsValid: true,
            Signature: null // Если мы парсили, то Signature здесь нет
        );
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
            Raw: signature.Raw
        );
    }
}

public class CSharpSyntaxWrapperTypeBuilderDefault : ICSharpSyntaxWrapperTypeBuilderDefault
{
    private readonly ISignatureParser<CSharpIdentifier> _signatureParser;

    public CSharpSyntaxWrapperTypeBuilderDefault(ISignatureParser<CSharpIdentifier> signatureParser) => _signatureParser = signatureParser;

    public CSharpSyntaxWrapperType BuildDefaultWrapper(ISyntaxWrapper syntaxWrapper)
    {
        var args = BuildArgumentsFromParsing(syntaxWrapper);
        return new CSharpSyntaxWrapperType(
            name: args.Name,
            @namespace: args.Namespace,
            spanEnd: args.SpanEnd,
            spanStart: args.SpanStart,
            fullName: args.FullName,
            identifier: args.Identifier,
            shortName: args.ShortName,
            isValid: args.IsValid,
            signature: args.Signature);
    }

    /// <summary>
    /// Собирает аргументы для конструктора, выполняя парсинг.
    /// </summary>
    private CSharpSyntaxWrapperTypeArguments BuildArgumentsFromParsing(ISyntaxWrapper syntaxWrapper)
    {
        var parsedSignature = _signatureParser.Parse(syntaxWrapper.FullName);

        // TODO: подмена namespace
        // Логика подмены перенесена сюда, как и планировалось в исходном коде
        var finalNamespace = syntaxWrapper.Namespace.Equals(parsedSignature.Namespace, StringComparison.InvariantCulture)
            ? parsedSignature.Namespace
            : syntaxWrapper.Namespace;

        var fullName = $"{finalNamespace}.{parsedSignature.ClassName}";

        return new CSharpSyntaxWrapperTypeArguments(
            Name: parsedSignature.ClassName,
            Namespace: finalNamespace,
            SpanEnd: syntaxWrapper.SpanEnd,
            SpanStart: syntaxWrapper.SpanStart,
            FullName: fullName,
            Identifier: fullName, // Identifier совпадает с FullName
            ShortName: parsedSignature.ClassName,
            IsValid: true,
            Signature: null // Если мы парсили, то Signature здесь нет
        );
    }
}

internal record CSharpSyntaxWrapperTypeArguments(
    string Name,
    string Namespace,
    int SpanEnd,
    int SpanStart,
    string FullName,
    string Identifier,
    string ShortName,
    bool IsValid,
    ISignature? Signature
);
