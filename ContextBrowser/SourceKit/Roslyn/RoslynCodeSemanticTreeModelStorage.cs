using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

//context: csharp, read, create, update, delete
public class RoslynCodeSemanticTreeModelStorage : ISemanticModelStorage
{
    private readonly Dictionary<string, StorageTreeModelItem> _cache = new();
    private readonly LinkedList<string> _usageOrder = new();
    private readonly int _capacity;
    private readonly object _sync = new();

    public RoslynCodeSemanticTreeModelStorage(int capacity = 0)
    {
        _capacity = capacity;
    }

    public bool IsInfinite => _capacity <= 0;

    //context: csharp, create
    public void Add(SyntaxTree tree, SemanticModel? model)
    {
        if(tree == null || model == null)
            return;

        string filePath = tree.FilePath;
        if(string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine($"[WARN] SyntaxTree has no file path; skipping storage");
            return;
        }

        lock(_sync)
        {
            if(_cache.ContainsKey(filePath))
                return;

            if(!IsInfinite && _cache.Count >= _capacity)
                DisposeOldest_NoLock();

            _cache[filePath] = new StorageTreeModelItem(tree, model);
            _usageOrder.AddLast(filePath);
        }
    }

    //context: csharp, create
    public void AddRange(IEnumerable<KeyValuePair<SyntaxTree, SemanticModel>> models)
    {
        foreach (var (tree, model) in models)
        {
            Add(tree, model);
        }
    }

    //context: csharp, read
    public SemanticModel? GetModel(SyntaxTree tree)
    {
        if(tree == null || string.IsNullOrWhiteSpace(tree.FilePath))
        {
            Console.WriteLine($"[WARN] SyntaxTree has no file path; skipping lookup");
            return null;
        }

        string filePath = tree.FilePath;
        lock(_sync)
        {
            if(!_cache.TryGetValue(filePath, out var item))
                return null;

            // делаем добавленный последним
            _usageOrder.Remove(filePath);
            _usageOrder.AddLast(filePath);
            return item.model;
        }
    }

    //context: csharp, read
    public IEnumerable<SyntaxTree> GetAllSyntaxTrees()
    {
        lock(_sync)
        {
            return _cache.Values.Select(v => v.tree).ToList();
        }
    }

    public void DisposeOldest()
    {
        lock(_sync)
        {
            DisposeOldest_NoLock();
        }
    }

    internal void DisposeOldest_NoLock()
    {
        if(_usageOrder.Count == 0)
            return;

        var oldest = _usageOrder.First!.Value;
        _usageOrder.RemoveFirst();
        _cache.Remove(oldest);
    }

    //context: csharp, delete
    public void Flush()
    {
        lock(_sync)
        {
            _cache.Clear();
            _usageOrder.Clear();
        }
    }
}

internal record struct StorageTreeModelItem(SyntaxTree tree, SemanticModel model)
{
    public static implicit operator (SyntaxTree tree, SemanticModel model)(StorageTreeModelItem value)
    {
        return (value.tree, value.model);
    }

    public static implicit operator StorageTreeModelItem((SyntaxTree tree, SemanticModel model) value)
    {
        return new StorageTreeModelItem(value.tree, value.model);
    }
}