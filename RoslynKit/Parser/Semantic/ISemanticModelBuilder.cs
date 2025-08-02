using Microsoft.CodeAnalysis;
using RoslynKit.Model;

namespace RoslynKit.Parser.Semantic;

// context: csharp, model
public interface ISemanticModelBuilder
{
    // context: csharp, model
    public ISemanticModelStorage ModelStorage { get; }

    // context: csharp, build
    public RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken);

    // context: csharp, build
    public Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options, CancellationToken cancellationToken);
}