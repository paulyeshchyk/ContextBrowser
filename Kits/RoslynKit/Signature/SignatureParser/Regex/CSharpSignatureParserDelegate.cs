using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserDelegate : ISignatureParserRegex
{
    private readonly ISignatureRegexMatcher _regexMatcher;
    private readonly IAppOptionsStore _optionsStore;

    public CSharpSignatureParserDelegate(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        _regexMatcher = regexMatcher;
        _optionsStore = optionsStore;
    }

    public SignatureParsingResult Parse(string input)
    {
        var match = _regexMatcher.MatchDelegateSignature(input);

        if (!match.Success)
        {
            return SignatureParsingResult.FailureResult();
        }

        // Для делегатов мы искусственно назначаем Namespace и ClassName из опций (FakeNaming)
        var semanticOptions = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;

        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: match.Groups["ResultType"].Value.Trim(),
            Namespace: semanticOptions.FakeNaming.NamespaceName,
            ClassName: "Delegate",
            MethodName: match.Groups["MethodName"].Value,
            Arguments: match.Groups["Arguments"].Value.Trim(),
            Raw: input));
    }
}
