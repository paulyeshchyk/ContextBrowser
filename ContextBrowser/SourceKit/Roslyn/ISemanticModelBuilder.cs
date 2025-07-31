using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, model
public interface ISemanticModelBuilder
{
    // context: csharp, model
    public ISemanticModelStorage ModelStorage { get; }

    // context: csharp, build
    public RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options);

    // context: csharp, build
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options);
}