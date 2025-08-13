namespace RoslynKit.Model.Storage;

// context: csharp, model
public interface ISemanticModelStorage<TSyntaxTree, TSemanticModel>
{
    // context: csharp, create
    void Add(TSyntaxTree syntaxTree, TSemanticModel? model);

    // context: csharp, create
    void AddRange(IEnumerable<KeyValuePair<TSyntaxTree, TSemanticModel>> models);

    // context: csharp, read
    TSemanticModel? GetModel(TSyntaxTree syntaxTree);

    // context: csharp, read
    IEnumerable<TSyntaxTree> GetAllSyntaxTrees();
}