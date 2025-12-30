using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextElementExtractor
{
    // Возвращает структурированный результат классификации
    ContextElementGroups Extract(ContextInfo item);
}

public class ContextElementGroups
{
    public List<string> Verbs { get; init; } = new List<string>();
    public List<string> Nouns { get; init; } = new List<string>();
}
