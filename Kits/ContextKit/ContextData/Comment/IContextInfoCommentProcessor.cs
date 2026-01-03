using System.Collections.Generic;

namespace ContextKit.ContextData.Comment;

// context: ContextInfo, build
public interface IContextInfoCommentProcessor<T>
{
    // context: ContextInfo, comment, build
    void Process(T? target, string comment);
}

public interface ICommentParsingStrategyFactory<TContext>
{
    IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies();
}