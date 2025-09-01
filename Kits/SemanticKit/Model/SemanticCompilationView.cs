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

public class SemanticCompilationMap : IEnumerable<CompilationMap>
{
    private readonly Dictionary<ISyntaxTreeWrapper, ISemanticModelWrapper> _data
        = new();

    public void Add(ISyntaxTreeWrapper syntaxTree, ISemanticModelWrapper semanticModel)
    {
        _data[syntaxTree] = semanticModel;
    }

    public void Add(CompilationMap map)
    {
        _data[map.SyntaxTree] = map.SemanticModel;
    }

    public void AddRange(IEnumerable<CompilationMap> models)
    {
        foreach (var m in models)
            _data[m.SyntaxTree] = m.SemanticModel;
    }

    public ISemanticModelWrapper? Get(ISyntaxTreeWrapper syntaxTree)
    {
        return _data.TryGetValue(syntaxTree, out var value) ? value : default;
    }

    public IEnumerable<ISyntaxTreeWrapper> GetAllSyntaxTrees()
        => _data.Keys;

    public IEnumerator<CompilationMap> GetEnumerator()
        => _data.Select(kvp => new CompilationMap(kvp.Key, kvp.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public record CompilationMap(ISyntaxTreeWrapper SyntaxTree, ISemanticModelWrapper SemanticModel);