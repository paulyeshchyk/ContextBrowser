using ContextKit.Model;

namespace SemanticKit.Model;

public interface ISyntaxWrapper : ISemanticInfo, ISpanInfo
{
    bool IsPartial { get; set; }

    bool IsValid { get; }

    ICustomMethodSignature? Signature { get; }
}