using System;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

public interface ISignatureParser<TIdentifier>
    where TIdentifier : ISignatureTypeIdentifier
{
    SignatureDefault Parse(string input);
}
