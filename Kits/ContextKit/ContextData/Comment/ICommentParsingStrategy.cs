namespace ContextKit.Stategies;

public interface ICommentParsingStrategy<T>
{
    void Execute(T? container, string comment);
}
