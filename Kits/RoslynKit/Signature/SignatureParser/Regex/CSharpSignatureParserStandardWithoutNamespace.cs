using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserStandardWithoutNamespace : ISignatureParserRegex
{
    private readonly ISignatureRegexMatcher _regexMatcher;
    private readonly IAppOptionsStore _optionsStore;

    public CSharpSignatureParserStandardWithoutNamespace(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        _regexMatcher = regexMatcher;
        _optionsStore = optionsStore;
    }

    public SignatureParsingResult Parse(string input)
    {
        var fullSignatureMatch = _regexMatcher.MatchFullSignature(input);

        if (!fullSignatureMatch.Success)
        {
            // failed with:
            // System.Collections.Generic.Dictionary<object, ContextKit.Model.ContextInfo>? IKeyIndexBuilder.Build(System.Collections.Generic.IEnumerable<ContextKit.Model.ContextInfo> contextsList)
            return SignatureParsingResult.FailureResult();
        }

        string typeAndClass = fullSignatureMatch.Groups["TypeAndClass"].Value;
        string resultType = fullSignatureMatch.Groups["ResultType"].Value.Trim();
        string methodName = fullSignatureMatch.Groups["MethodName"].Value;
        string arguments = fullSignatureMatch.Groups["Arguments"].Value.Trim();

        // *Проверяем условие:* разделение НЕ должно быть успешным (т.е., нет точки)
        var typeSplitMatch = _regexMatcher.SplitTypeAndClass(typeAndClass);
        if (typeSplitMatch.Success)
        {
            // Если точка найдена, это сценарий для StandardSignatureWithNamespaceParser.
            return SignatureParsingResult.FailureResult();
        }
        var semanticOptions = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;
        // 5. Если TypeAndClass не содержит точки (например, просто "Console").
        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: resultType,
            Namespace: semanticOptions.ExternalNaming.NamespaceName,
            ClassName: typeAndClass,
            MethodName: methodName,
            Arguments: arguments,
            Raw: input));
    }
}