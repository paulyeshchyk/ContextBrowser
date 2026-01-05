using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ContextKit.Model;

public interface IContextCollector<T>
{
    void Add(T item);

    bool AddIfNotExists(T item);

    T? GetItem(string fullName);

    void Append(T item);

    void Append(T item, T owns);

    IEnumerable<T> GetAll();

    void MergeFakeItems();

    ConcurrentDictionary<string, T> Collection { get; }

    void Renew(IEnumerable<T> byItems);
}
