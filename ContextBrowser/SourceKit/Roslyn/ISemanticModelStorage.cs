using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, model
public interface ISemanticModelStorage
{
    // context: csharp, create
    void Add(SyntaxTree syntaxTree, SemanticModel? model);

    // context: csharp, create
    void AddRange(IEnumerable<KeyValuePair<SyntaxTree, SemanticModel>> models);

    // context: csharp, read
    SemanticModel? GetModel(SyntaxTree syntaxTree);

    // context: csharp, read
    IEnumerable<SyntaxTree> GetAllSyntaxTrees();
}