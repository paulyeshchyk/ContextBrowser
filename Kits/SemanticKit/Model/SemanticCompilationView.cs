using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SemanticKit.Model;

//context: semantic, model
public record struct SemanticCompilationView(ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object/*CompilationUnitSyntax*/ unitSyntax)
{
    public static implicit operator (ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object unitSyntax)(SemanticCompilationView value)
    {
        return (value.model, value.tree, value.unitSyntax);
    }

    public static implicit operator SemanticCompilationView((ISemanticModelWrapper model, ISyntaxTreeWrapper tree, object unitSyntax) value)
    {
        return new SemanticCompilationView(value.model, value.tree, value.unitSyntax);
    }
}

public class SemanticCompilationMap<TSyntaxTreeWrapper> : IEnumerable<CompilationMap<TSyntaxTreeWrapper>>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly Dictionary<TSyntaxTreeWrapper, ISemanticModelWrapper> _data
        = new();

    public SemanticCompilationMap()
    {
        _data = new Dictionary<TSyntaxTreeWrapper, ISemanticModelWrapper>();
    }

    public SemanticCompilationMap(IEnumerable<CompilationMap<TSyntaxTreeWrapper>> maps)
    {
        _data = maps.ToDictionary(
            map => map.SyntaxTree,
            map => map.SemanticModel);
    }

    public void Add(TSyntaxTreeWrapper syntaxTree, ISemanticModelWrapper semanticModel)
    {
        _data[syntaxTree] = semanticModel;
    }

    public void Add(CompilationMap<TSyntaxTreeWrapper> map)
    {
        _data[map.SyntaxTree] = map.SemanticModel;
    }

    public void AddRange(IEnumerable<CompilationMap<TSyntaxTreeWrapper>> models)
    {
        foreach (var m in models)
            _data[m.SyntaxTree] = m.SemanticModel;
    }

    public ISemanticModelWrapper? Get(TSyntaxTreeWrapper syntaxTree)
    {
        return _data.TryGetValue(syntaxTree, out var value) ? value : default;
    }

    public IEnumerable<TSyntaxTreeWrapper> GetAllSyntaxTrees()
        => _data.Keys;

    public IEnumerator<CompilationMap<TSyntaxTreeWrapper>> GetEnumerator()
        => _data.Select(kvp => new CompilationMap<TSyntaxTreeWrapper>(kvp.Key, kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public record CompilationMap<TSyntaxTreeWrapper>(TSyntaxTreeWrapper SyntaxTree, ISemanticModelWrapper SemanticModel)
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper;