using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

public interface ISemanticModelStorage
{
    void Add(SyntaxTree syntaxTree, SemanticModel? model);

    void AddRange(IEnumerable<KeyValuePair<SyntaxTree, SemanticModel>> models);

    SemanticModel? GetModel(SyntaxTree syntaxTree);

    IEnumerable<SyntaxTree> GetAllSyntaxTrees();
}