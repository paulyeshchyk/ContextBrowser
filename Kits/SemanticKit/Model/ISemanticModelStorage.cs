namespace SemanticKit.Model;

// context: csharp, model
public interface ISemanticModelStorage<ISyntaxTreeWrapper, TSemanticModel>
{
    // context: csharp, create
    void Add(ISyntaxTreeWrapper syntaxTree, TSemanticModel? model);

    // context: csharp, create
    void AddRange(IEnumerable<KeyValuePair<ISyntaxTreeWrapper, TSemanticModel>> models);

    // context: csharp, read
    TSemanticModel? GetModel(ISyntaxTreeWrapper syntaxTree);

    // context: csharp, read
    IEnumerable<ISyntaxTreeWrapper> GetAllSyntaxTrees();
}