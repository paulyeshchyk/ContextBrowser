using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextElementExtractor<TContext>
    where TContext : IContextWithReferences<TContext>
{
    // Возвращает структурированный результат классификации
    ContextElementGroups Extract(TContext item);
}

public class ContextElementGroups
{
    public List<string> Verbs { get; init; } = new List<string>();
    public List<string> Nouns { get; init; } = new List<string>();
}
