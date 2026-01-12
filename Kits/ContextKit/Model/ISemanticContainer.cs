namespace ContextKit.Model;

public interface ISemanticContainer
{
    ContextInfoElementType ElementType { get; set; }

    ContentInfoElementVisibility ElementVisibility { get; set; }

    public ISymbolInfo? SymbolWrapper { get; set; }

    public ISyntaxNodeWrapper? SyntaxWrapper { get; set; }
}
