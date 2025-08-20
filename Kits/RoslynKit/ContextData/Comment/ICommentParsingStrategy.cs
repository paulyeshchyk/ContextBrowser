namespace RoslynKit.ContextData.Comment;

public interface ICommentParsingStrategy<T>
{
    void Execute(T? container, string comment);
}
