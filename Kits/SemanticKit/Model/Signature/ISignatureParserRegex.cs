using System;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

public interface ISignatureParserRegex
{
    SignatureParsingResult Parse(string input);
}
