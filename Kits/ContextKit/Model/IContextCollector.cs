using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextCollector<T>
{
    void Add(T item);

    void Append(T item);

    IEnumerable<T> GetAll();

    void MergeFakeItems();

    Dictionary<string, T> BySymbolDisplayName { get; }

    void Renew(IEnumerable<T> byItems);
}
