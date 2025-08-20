using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: csharp, model
public interface ISemanticModelBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : notnull
    where TSemanticModel : notnull
{
    // context: csharp, model
    public ISemanticModelStorage<TSyntaxTree, TSemanticModel> ModelStorage { get; }

    // context: csharp, build
    public RoslynCompilationView BuildModel(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    // context: csharp, build
    public Dictionary<TSyntaxTree, TSemanticModel> BuildModels(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);
}