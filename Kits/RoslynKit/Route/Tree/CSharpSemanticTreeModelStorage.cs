using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using SemanticKit.Model;

namespace RoslynKit.Route.Tree;

//context: roslyn, read, create, update, delete
public class CSharpSemanticTreeModelStorage : ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper>
{
    private OnWriteLog? _onWriteLog;
    private readonly Dictionary<string, StorageTreeModelItem> _cache = new();
    private readonly LinkedList<string> _usageOrder = new();
    private readonly int _capacity;
    private readonly object _sync = new();

    public CSharpSemanticTreeModelStorage(int capacity, OnWriteLog? onWriteLog)
    {
        _capacity = capacity;
        _onWriteLog = onWriteLog;
    }

    public bool IsInfinite => _capacity <= 0;

    //context: roslyn, create
    public void Add(ISyntaxTreeWrapper tree, ISemanticModelWrapper? model)
    {
        if (tree == null || model == null)
            return;

        string filePath = tree.FilePath;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "[WARN] SyntaxTree has no file path; skipping storage");
            return;
        }

        lock (_sync)
        {
            if (_cache.ContainsKey(filePath))
                return;

            if (!IsInfinite && _cache.Count >= _capacity)
                DisposeOldest_NoLock();

            _cache[filePath] = new StorageTreeModelItem(tree, model);
            _usageOrder.AddLast(filePath);
        }
    }

    //context: roslyn, create
    public void AddRange(IEnumerable<KeyValuePair<ISyntaxTreeWrapper, ISemanticModelWrapper>> models)
    {
        foreach (var (tree, model) in models)
        {
            Add(tree, model);
        }
    }

    //context: roslyn, read
    public ISemanticModelWrapper? GetModel(ISyntaxTreeWrapper tree)
    {
        if (tree == null || string.IsNullOrWhiteSpace(tree.FilePath))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "[WARN] SyntaxTree has no file path; skipping lookup");
            return null;
        }

        string filePath = tree.FilePath;
        lock (_sync)
        {
            if (!_cache.TryGetValue(filePath, out var item))
                return null;

            // ������ ����������� ���������
            _usageOrder.Remove(filePath);
            _usageOrder.AddLast(filePath);
            return item.model;
        }
    }

    //context: roslyn, read
    public IEnumerable<ISyntaxTreeWrapper> GetAllSyntaxTrees()
    {
        lock (_sync)
        {
            return _cache.Values.Select(v => v.tree).ToList();
        }
    }

    public void DisposeOldest()
    {
        lock (_sync)
        {
            DisposeOldest_NoLock();
        }
    }

    internal void DisposeOldest_NoLock()
    {
        if (_usageOrder.Count == 0)
            return;

        var oldest = _usageOrder.First!.Value;
        _usageOrder.RemoveFirst();
        _cache.Remove(oldest);
    }

    //context: roslyn, delete
    public void Flush()
    {
        lock (_sync)
        {
            _cache.Clear();
            _usageOrder.Clear();
        }
    }
}

internal record struct StorageTreeModelItem(ISyntaxTreeWrapper tree, ISemanticModelWrapper model)
{
    public static implicit operator (ISyntaxTreeWrapper tree, ISemanticModelWrapper model)(StorageTreeModelItem value)
    {
        return (value.tree, value.model);
    }

    public static implicit operator StorageTreeModelItem((ISyntaxTreeWrapper tree, ISemanticModelWrapper model) value)
    {
        return new StorageTreeModelItem(value.tree, value.model);
    }
}