using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserFake : ISignatureParserRegex
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly ISignatureRegexMatcher _regexMatcher;

    public CSharpSignatureParserFake(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        _regexMatcher = regexMatcher;
        _optionsStore = optionsStore;
    }

    public SignatureParsingResult Parse(string input)
    {

        var _semanticOptions = _optionsStore.GetOptions<CodeParsingOptions>().SemanticOptions;
        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: _semanticOptions.FakeNaming.ResultTypeName,
            Namespace: _semanticOptions.FakeNaming.NamespaceName,
            ClassName: _semanticOptions.FakeNaming.ClassName,
            MethodName: input,
            Arguments: string.Empty,
            Raw: input));
    }
}
