using ContextKit.Model;

namespace SemanticKit.Model;

public interface BaseSyntaxWrapper : ISemanticInfo, ISpanInfo
{
    bool IsPartial { get; set; }

    bool IsValid { get; }
}