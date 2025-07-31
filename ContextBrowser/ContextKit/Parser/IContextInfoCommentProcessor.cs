namespace ContextBrowser.ContextKit.Parser;

// context: contextInfo, build
public interface IContextInfoCommentProcessor<T>
{
    // context: contextInfo, build
    void Process(string comment, T target);
}
