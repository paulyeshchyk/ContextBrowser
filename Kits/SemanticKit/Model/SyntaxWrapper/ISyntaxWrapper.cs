using ContextKit.Model;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.SyntaxWrapper;

public interface ISyntaxWrapper : ISemanticInfo, ISpanInfo
{
    bool IsValid { get; }

    ISignature? Signature { get; }

    IContextInfo GetContextInfoDto();
}