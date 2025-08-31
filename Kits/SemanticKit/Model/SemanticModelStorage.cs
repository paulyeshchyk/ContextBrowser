using ContextBrowserKit;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;

namespace SemanticKit.Model;

//context: roslyn, read, create, update, delete
public class SemanticModelStorage : ISemanticModelStorage<ISyntaxTreeWrapper, ISemanticModelWrapper>
{
    private readonly LRUCache<string, CompilationMap> _cache;
    private readonly IAppLogger<AppLevel> _logger;

    public SemanticModelStorage(IAppLogger<AppLevel> logger)
    {
        int capacity = 0;
        _cache = new LRUCache<string, CompilationMap>(capacity);
        _logger = logger;
    }

    public bool IsInfinite => _cache.IsInfinite;

    public void Add(ISyntaxTreeWrapper tree, ISemanticModelWrapper? model)
    {
        if (!IsValid(tree, model, out var filePath))
            return;

        _cache.Add(filePath, new CompilationMap(tree, model!));
    }

    public void AddRange(IEnumerable<CompilationMap> models)
    {
        foreach (var map in models)
            Add(map.SyntaxTree, map.SemanticModel);
    }

    public void Add(SemanticCompilationMap compilationMap)
        => AddRange(compilationMap);

    public ISemanticModelWrapper? GetModel(ISyntaxTreeWrapper tree)
    {
        if (!IsValid(tree, null, out var filePath))
            return null;

        return _cache.TryGetValue(filePath, out var map) ? map?.SemanticModel : null;
    }

    public IEnumerable<ISyntaxTreeWrapper> GetAllSyntaxTrees()
        => _cache.Values.Select(v => v.SyntaxTree);

    public void Flush() => _cache.Clear();

    private bool IsValid(ISyntaxTreeWrapper? tree, ISemanticModelWrapper? model, out string filePath)
    {
        filePath = tree?.FilePath ?? string.Empty;

        if (tree == null || string.IsNullOrWhiteSpace(filePath) || model == null && !_cache.TryGetValue(filePath, out _))
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, "[WARN] SyntaxTree has no file path; skipping");
            return false;
        }

        return true;
    }

    public void AddRange(IEnumerable<KeyValuePair<ISyntaxTreeWrapper, ISemanticModelWrapper>> models)
    {
        foreach (var model in models)
        {
            Add(model.Key, model.Value);
        }
    }
}