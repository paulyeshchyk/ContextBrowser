namespace RoslynKit.ContextData.Comment;

// context: contextInfo, build
public interface IContextInfoCommentProcessor<T>
{
    // context: contextInfo, comment, build
    void Process(T? target, string comment);
}