using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserStandardWithNamespace : ISignatureParserRegex
{
    private readonly ISignatureRegexMatcher _regexMatcher;
    private readonly IAppOptionsStore _optionsStore;

    public CSharpSignatureParserStandardWithNamespace(ISignatureRegexMatcher regexMatcher, IAppOptionsStore optionsStore)
    {
        _regexMatcher = regexMatcher;
        _optionsStore = optionsStore;
    }

    public SignatureParsingResult Parse(string input)
    {
        // 1. Пытаемся матчить всю сигнатуру (ResultType + FullName + Args)
        var fullSignatureMatch = _regexMatcher.MatchFullSignature(input);

        if (!fullSignatureMatch.Success)
        {
            // failed with:
            // System.Collections.Generic.Dictionary<object, ContextKit.Model.ContextInfo>? IKeyIndexBuilder.Build(System.Collections.Generic.IEnumerable<ContextKit.Model.ContextInfo> contextsList)
            return SignatureParsingResult.FailureResult();
        }

        // 2. Успех первого этапа. Теперь разбираем TypeAndClass
        string typeAndClass = fullSignatureMatch.Groups["TypeAndClass"].Value;
        string resultType = fullSignatureMatch.Groups["ResultType"].Value.Trim();

        // 3. Используем новый метод для разделения (ищем точку)
        var typeSplitMatch = _regexMatcher.SplitTypeAndClass(typeAndClass);

        // 4. Если разделение прошло УСПЕШНО (т.е. Namespace есть)
        if (!typeSplitMatch.Success)
        {
            // failed with:
            // System.Collections.Generic.IEnumerable<ContextKit.Model.ContextInfo> Enumerable.Where<ContextKit.Model.ContextInfo>(...)
            return SignatureParsingResult.FailureResult();
        }

        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: resultType,
            Namespace: typeSplitMatch.Groups["Namespace"].Value,
            ClassName: typeSplitMatch.Groups["ClassName"].Value,
            MethodName: fullSignatureMatch.Groups["MethodName"].Value,
            Arguments: fullSignatureMatch.Groups["Arguments"].Value.Trim(),
            Raw: input));
    }
}
