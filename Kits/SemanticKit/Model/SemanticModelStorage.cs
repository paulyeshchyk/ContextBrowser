using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;

namespace SemanticKit.Model;

//context: roslyn, read, create, update, delete
public class SemanticModelStorage<TSyntaxTreeWrapper> : ISemanticModelStorage<TSyntaxTreeWrapper, ISemanticModelWrapper>
    where TSyntaxTreeWrapper: ISyntaxTreeWrapper
{
    private readonly LRUCache<string, CompilationMap<TSyntaxTreeWrapper>> _cache;
    private readonly IAppLogger<AppLevel> _logger;

    public SemanticModelStorage(IAppLogger<AppLevel> logger)
    {
        int capacity = 0;
        _cache = new LRUCache<string, CompilationMap<TSyntaxTreeWrapper>>(capacity);
        _logger = logger;
    }

    public bool IsInfinite => _cache.IsInfinite;

    public void Add(TSyntaxTreeWrapper tree, ISemanticModelWrapper? model)
    {
        if (!IsValid(tree, model, out var filePath))
            return;

        _cache.Add(filePath, new CompilationMap<TSyntaxTreeWrapper>(tree, model!));
    }

    public void AddRange(IEnumerable<CompilationMap<TSyntaxTreeWrapper>> models)
    {
        foreach (var map in models)
            Add(map.SyntaxTree, map.SemanticModel);
    }

    public void Add(SemanticCompilationMap<TSyntaxTreeWrapper> compilationMap)
        => AddRange(compilationMap);

    public ISemanticModelWrapper? GetModel(TSyntaxTreeWrapper tree)
    {
        if (!IsValid(tree, null, out var filePath))
            return null;

        return _cache.TryGetValue(filePath, out var map) ? map?.SemanticModel : null;
    }

    public IEnumerable<TSyntaxTreeWrapper> GetAllSyntaxTrees()
        => _cache.Values.Select(v => v.SyntaxTree);

    public void Flush() => _cache.Clear();

    private bool IsValid(TSyntaxTreeWrapper? tree, ISemanticModelWrapper? model, out string filePath)
    {
        filePath = tree?.FilePath ?? string.Empty;

        if (tree == null || string.IsNullOrWhiteSpace(filePath) || model == null && !_cache.TryGetValue(filePath, out _))
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Warn, "[WARN] SyntaxTree has no file path; skipping");
            return false;
        }

        return true;
    }

    public void AddRange(IEnumerable<KeyValuePair<TSyntaxTreeWrapper, ISemanticModelWrapper>> models)
    {
        foreach (var model in models)
        {
            Add(model.Key, model.Value);
        }
    }
}