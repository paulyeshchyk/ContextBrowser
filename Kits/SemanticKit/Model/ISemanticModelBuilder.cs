using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticModelBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : notnull
    where TSemanticModel : notnull
{
    // context: semantic, model
    public ISemanticModelStorage<TSyntaxTree, TSemanticModel> ModelStorage { get; }

    // context: semantic, build
    public RoslynCompilationView BuildModel(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    // context: semantic, build
    public Dictionary<TSyntaxTree, TSemanticModel> BuildModels(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);
}