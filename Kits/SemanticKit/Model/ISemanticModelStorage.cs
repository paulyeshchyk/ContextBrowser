namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticModelStorage<ISyntaxTreeWrapper, TSemanticModel>
{
    // context: semantic, create
    void Add(ISyntaxTreeWrapper syntaxTree, TSemanticModel? model);

    // context: semantic, create
    void AddRange(IEnumerable<KeyValuePair<ISyntaxTreeWrapper, TSemanticModel>> models);

    // context: semantic, read
    TSemanticModel? GetModel(ISyntaxTreeWrapper syntaxTree);

    // context: semantic, read
    IEnumerable<ISyntaxTreeWrapper> GetAllSyntaxTrees();
}