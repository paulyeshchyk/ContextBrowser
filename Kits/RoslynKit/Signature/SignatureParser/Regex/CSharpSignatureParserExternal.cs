using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserExternal : ISignatureParserRegex
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly ISignatureRegexMatcher _regexMatcher;

    public CSharpSignatureParserExternal(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        _regexMatcher = regexMatcher;
        _optionsStore = optionsStore;
    }

    public SignatureParsingResult Parse(string input)
    {
        var m2 = _regexMatcher.MatchNamespaceAndClass(input);

        // Если m2 успешно нашел класс и метод
        if (!m2.Success)
        {
            // failed with:
            // System.Collections.Generic.Dictionary<object, ContextKit.Model.ContextInfo>? IKeyIndexBuilder.Build(System.Collections.Generic.IEnumerable<ContextKit.Model.ContextInfo> contextsList)

            return SignatureParsingResult.FailureResult();
        }

        var _semanticOptions = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;

        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: _semanticOptions.ExternalNaming.ResultTypeName,
            Namespace: m2.Groups[1].Value,
            ClassName: m2.Groups[2].Value,
            MethodName: _semanticOptions.ExternalNaming.MethodName,
            Arguments: string.Empty,
            Raw: input));
    }
}
