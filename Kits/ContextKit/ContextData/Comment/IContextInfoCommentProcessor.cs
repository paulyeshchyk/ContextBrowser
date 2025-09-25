using System.Collections.Generic;

namespace ContextKit.ContextData.Comment;

// context: contextInfo, build
public interface IContextInfoCommentProcessor<T>
{
    // context: contextInfo, comment, build
    void Process(T? target, string comment);
}

public interface ICommentParsingStrategyFactory<TContext>
{
    IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies();
}