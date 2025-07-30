using Microsoft.CodeAnalysis;

namespace ContextBrowser.SourceKit.Roslyn;

public interface ISemanticModelBuilder
{
    ISemanticModelStorage ModelStorage { get; }

    RoslynCompilationView BuildModel(string code, string filePath, RoslynCodeParserOptions options);

    Dictionary<SyntaxTree, SemanticModel> BuildModels(IEnumerable<string> codeFiles, RoslynCodeParserOptions options);
}