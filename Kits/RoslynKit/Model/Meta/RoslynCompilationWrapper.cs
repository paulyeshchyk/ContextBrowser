using Microsoft.CodeAnalysis;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

// context: roslyn, compilation, build
public class RoslynCompilationWrapper : ICompilationWrapper
{
    private readonly Compilation _compilation;

    public RoslynCompilationWrapper(Compilation compilation)
    {
        _compilation = compilation;
    }

    // context: roslyn, compilation, build
    public ISemanticModelWrapper GetSemanticModel(ISyntaxTreeWrapper wrapper)
    {
        var semanticModel = _compilation.GetSemanticModel((SyntaxTree)wrapper.Tree);
        return new RoslynSemanticModelWrapper(semanticModel);
    }
}

