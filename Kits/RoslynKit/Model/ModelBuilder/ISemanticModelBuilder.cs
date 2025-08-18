using RoslynKit.Model.Storage;

namespace RoslynKit.Model.ModelBuilder;

// context: csharp, model
public interface ISemanticModelBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : notnull
    where TSemanticModel : notnull
{
    // context: csharp, model
    public ISemanticModelStorage<TSyntaxTree, TSemanticModel> ModelStorage { get; }

    // context: csharp, build
    public RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken);

    // context: csharp, build
    public Dictionary<TSyntaxTree, TSemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, CancellationToken cancellationToken);
}