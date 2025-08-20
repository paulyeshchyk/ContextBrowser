namespace SemanticKit.Model;

public interface ICompilationBuilder
{
    ICompilationWrapper BuildCompilation(IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name);

    Dictionary<ISyntaxTreeWrapper, ISemanticModelWrapper> BuildModels(IEnumerable<string> codeFiles, CancellationToken cancellationToken = default);
}
