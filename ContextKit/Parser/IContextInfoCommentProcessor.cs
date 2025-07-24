namespace ContextBrowser.ContextKit.Parser;

public interface IContextInfoCommentProcessor<T>
{
    void Process(string comment, T target);
}
