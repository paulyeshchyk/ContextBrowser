using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextBrowserKit;

public class LRUCache<TKey, TValue>
    where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _map;
    private readonly LinkedList<(TKey Key, TValue Value)> _order;
    private readonly object _sync = new();

    public LRUCache(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");

        _capacity = capacity;
        _map = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();
        _order = new LinkedList<(TKey, TValue)>();
    }

    public bool IsInfinite => _capacity == 0;

    public int Count
    {
        get { lock (_sync) return _map.Count; }
    }

    public void Add(TKey key, TValue value)
    {
        lock (_sync)
        {
            if (_map.TryGetValue(key, out var existing))
            {
                _order.Remove(existing);
                _map.Remove(key);
            }
            else if (!IsInfinite && _map.Count >= _capacity)
            {
                RemoveOldest_NoLock();
            }

            var node = new LinkedListNode<(TKey, TValue)>((key, value));
            _order.AddLast(node);
            _map[key] = node;
        }
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        lock (_sync)
        {
            if (_map.TryGetValue(key, out var node))
            {
                // обновляем порядок (LRU → MRU)
                _order.Remove(node);
                _order.AddLast(node);
                value = node.Value.Value;
                return true;
            }

            value = default;
            return false;
        }
    }

    public IEnumerable<TKey> Keys
    {
        get { lock (_sync) return _map.Keys.ToList(); }
    }

    public IEnumerable<TValue> Values
    {
        get { lock (_sync) return _map.Values.Select(n => n.Value.Value).ToList(); }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _map.Clear();
            _order.Clear();
        }
    }

    private void RemoveOldest_NoLock()
    {
        if (_order.First is { } oldest)
        {
            _map.Remove(oldest.Value.Key);
            _order.RemoveFirst();
        }
    }
}
