using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Collector;

// context: ContextInfo, build
public class ContextInfoCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public ConcurrentDictionary<string, TContext> Collection { get; } = new(StringComparer.Ordinal);
    private readonly ConcurrentBag<TContext> FakeCollection = new ConcurrentBag<TContext>();

    private readonly object _lock = new();

    public ContextInfoCollector()
    {
    }

    // context: ContextInfo, read
    public IEnumerable<TContext> GetAll() => Collection.Values;

    // context: ContextInfo, build
    public bool AddIfNotExists(TContext info)
    {
        // TryAdd атомарно добавляет, только если ключ отсутствует, возвращая false, если уже есть.
        if (string.IsNullOrWhiteSpace(info.FullName))
        {
            // Здесь может потребоваться lock, если вы хотите синхронизировать доступ к Collection (HashSet)
            // Но мы заменили HashSet на ConcurrentDictionary, поэтому FullName должен быть ключом.
            return false; // или добавить в FakeCollection
        }

        return Collection.TryAdd(info.FullName, info);
    }

    public TContext? GetItem(string predicate)
    {
        Collection.TryGetValue(predicate, out TContext? result);
        return result;
    }

    // context: ContextInfo, build
    public void Add(TContext info)
    {
        if (string.IsNullOrWhiteSpace(info.FullName))
            return;

        Collection.AddOrUpdate(info.FullName, info, (key, existing) => info);
    }

    public void Append(TContext info)
    {
        FakeCollection.Add(info);

        if (!string.IsNullOrWhiteSpace(info.FullName))
            Collection.AddOrUpdate(info.FullName, info, (key, existing) => info);
    }

    public void Append(TContext item, TContext owns)
    {
        lock (_lock)
        {
            item.Owns.Add(owns);

            FakeCollection.Add(item);

            if (!string.IsNullOrWhiteSpace(item.FullName))
                Collection.AddOrUpdate(item.FullName, item, (key, existing) => item);
        }
    }

    public void MergeFakeItems()
    {
        lock (_lock)
        {
            // 1. Извлекаем все элементы потокобезопасно
            List<TContext> itemsToMerge = new List<TContext>();
            while (FakeCollection.TryTake(out var item))
            {
                if (!string.IsNullOrWhiteSpace(item.FullName))
                {
                    itemsToMerge.Add(item);
                }
            }

            // 2. Слияние без lock, используя атомарные операции
            foreach (var item in itemsToMerge)
            {
                // TryAdd не требует блокировки, выполняется атомарно
                Collection.TryAdd(item.FullName, item);
            }
        }
    }

    public void Renew(IEnumerable<TContext> byItems)
    {
        // намеренно ничего не делаем — коллектор необновляемый здесь
    }

}

public class ContextInfoReferenceCollector<TContext> : IContextCollector<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public ConcurrentDictionary<string, TContext> Collection { get; } = new ConcurrentDictionary<string, TContext>();

    private readonly ConcurrentBag<TContext> FakeCollection = new ConcurrentBag<TContext>();

    private readonly ConcurrentBag<TContext> _existing = new ConcurrentBag<TContext>();
    private readonly object _lock = new();


    public ContextInfoReferenceCollector(IEnumerable<TContext> existing)
    {
        Renew(existing);
    }

    public void Renew(IEnumerable<TContext> byItems)
    {
        lock (_lock)
        {
            _existing.Clear();
            Collection.Clear();

            if (byItems.Any())
            {
                foreach (var item in byItems)
                    _existing.Add(item);


                var names = ContextInfoFullNameIndexBuilder.Build(_existing);
                foreach (var key in names.Keys)
                    Collection[key] = names[key];
            }
        }
    }

    public TContext? GetItem(string predicate)
    {
        Collection.TryGetValue(predicate, out TContext? result);
        return result;
    }

    public bool AddIfNotExists(TContext item)
    {
        // намеренно ничего не делаем — коллектор только читает
        return true;
    }

    public void Add(TContext item)
    {
        // намеренно ничего не делаем — коллектор только читает
    }

    public IEnumerable<TContext> GetAll() => _existing;

    public void Append(TContext info)
    {
        FakeCollection.Add(info);
        if (!string.IsNullOrWhiteSpace(info.FullName))
            Collection[info.FullName] = info;
    }

    public void Append(TContext item, TContext owns)
    {
        lock (_lock)
        {
            item.Owns.Add(owns);
            FakeCollection.Add(item);
            if (!string.IsNullOrWhiteSpace(item.FullName))
                Collection[item.FullName] = item;
        }
    }

    public void MergeFakeItems()
    {
        lock (_lock)
        {
            if (FakeCollection.IsEmpty) return;

            // Шаг 1: Извлекаем все элементы из ConcurrentBag
            List<TContext> incomingItems = new List<TContext>();
            while (FakeCollection.TryTake(out var item))
            {
                if (!string.IsNullOrWhiteSpace(item.FullName))
                {
                    incomingItems.Add(item);
                }
            }

            // Шаг 2: Создаем Хеш-индекс для существующего списка (_existing)
            // O(N) — это быстро
            // Поскольку _existing модифицируется только в этом lock и в Renew, мы можем это сделать.
            HashSet<string> existingFullNames = new HashSet<string>(_existing.Select(e => e.FullName), StringComparer.Ordinal);

            // Шаг 3: Фильтрация и добавление новых элементов
            // O(M) — это быстро
            foreach (var item in incomingItems)
            {
                // Проверка по индексу O(1)
                if (!existingFullNames.Contains(item.FullName))
                {
                    _existing.Add(item); // Теперь это ConcurrentBag.Add
                    existingFullNames.Add(item.FullName);
                }
            }

            // ВНИМАНИЕ: Мы также должны обновить Collection (ConcurrentDictionary, который служит индексом поиска)
            // Создадим новый индекс только для новых элементов
            foreach (var item in incomingItems)
            {
                Collection.TryAdd(item.FullName, item);
            }
        }
    }
}
