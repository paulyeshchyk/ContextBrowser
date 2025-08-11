namespace RoslynKit.Syntax.Parser.Comment;

// context: contextInfo, build
public interface IContextInfoCommentProcessor<T>
{
    // context: contextInfo, comment, build
    void Process(string comment, T target);
}