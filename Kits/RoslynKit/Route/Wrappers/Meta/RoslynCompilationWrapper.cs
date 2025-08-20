using Microsoft.CodeAnalysis;
using SemanticKit.Model;

namespace RoslynKit.Route.Wrappers.Meta;

public class RoslynCompilationWrapper : ICompilationWrapper
{
    private readonly Compilation _compilation;

    public RoslynCompilationWrapper(Compilation compilation)
    {
        _compilation = compilation;
    }

    public ISemanticModelWrapper GetSemanticModel(ISyntaxTreeWrapper wrapper)
    {
        var semanticModel = _compilation.GetSemanticModel((SyntaxTree)wrapper.Tree);
        return new RoslynSemanticModelWrapper(semanticModel);
    }
}

