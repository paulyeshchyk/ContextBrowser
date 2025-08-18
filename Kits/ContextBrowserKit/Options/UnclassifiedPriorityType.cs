namespace ContextBrowserKit.Options;

// context: model, contextInfo
public enum UnclassifiedPriorityType
{
    None,     // как сейчас
    Highest,  // NoAction / NoDomain идут первыми
    Lowest    // NoAction / NoDomain идут последними
}
