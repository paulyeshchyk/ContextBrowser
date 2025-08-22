namespace SemanticKit.Model;

public interface ICompilationBuilder
{
    ICompilationWrapper BuildCompilation(IEnumerable<ISyntaxTreeWrapper> syntaxTrees, IEnumerable<string> customAssembliesPaths, string name);

    SemanticCompilationMap BuildCompilationMap(IEnumerable<string> codeFiles, CancellationToken cancellationToken = default);
}
