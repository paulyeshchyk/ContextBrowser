using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, model
public interface ISemanticTreeModelBuilder<TSyntaxTree, TSemanticModel>
    where TSyntaxTree : notnull
    where TSemanticModel : notnull
{
    // context: semantic, model
    public ISemanticModelStorage<TSyntaxTree, TSemanticModel> SemanticTreeModelStorage { get; }

    // context: semantic, model
    public ISyntaxTreeWrapperBuilder SyntaxTreeWrapperBuilder { get; }

    // context: semantic, build
    public SemanticCompilationView BuildCompilationView(string code, string filePath, SemanticOptions options, CancellationToken cancellationToken);

    // context: semantic, build
    public SemanticCompilationMap BuildCompilationMap(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken);
}