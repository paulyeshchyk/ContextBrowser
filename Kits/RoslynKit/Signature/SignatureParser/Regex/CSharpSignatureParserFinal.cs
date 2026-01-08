using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureParser;

public class CSharpSignatureParserFinal : ISignatureParserRegex
{
    // Этот парсер не нуждается в Matcher или Options, но для единообразия оставим конструктор.
    public CSharpSignatureParserFinal() { }

    public SignatureParsingResult Parse(string input)
    {
        return SignatureParsingResult.SuccessResult(new SignatureDefault(
            ResultType: "Error",
            Namespace: "Error",
            ClassName: "Error",
            MethodName: "Error",
            Arguments: string.Empty,
            Raw: input));
    }
}
