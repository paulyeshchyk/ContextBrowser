using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Classifier;

namespace ContextKit.Stategies;

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