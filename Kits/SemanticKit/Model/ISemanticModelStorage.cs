using System.Collections.Generic;

namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticModelStorage<TSyntaxTreeWrapper, TSemanticModel>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    // context: semantic, create
    void Add(TSyntaxTreeWrapper syntaxTree, TSemanticModel? model);

    // context: semantic, create
    void AddRange(IEnumerable<KeyValuePair<TSyntaxTreeWrapper, TSemanticModel>> models);

    // context: semantic, create
    void Add(SemanticCompilationMap<TSyntaxTreeWrapper> map);

    // context: semantic, read
    TSemanticModel? GetModel(TSyntaxTreeWrapper syntaxTree);

    // context: semantic, read
    IEnumerable<TSyntaxTreeWrapper> GetAllSyntaxTrees();
}