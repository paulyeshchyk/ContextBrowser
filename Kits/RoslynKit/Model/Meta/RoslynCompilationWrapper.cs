using System.Collections.Immutable;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Meta;

// context: roslyn, compilation, build
public class RoslynCompilationWrapper : ICompilationWrapper
{
    private readonly IAppLogger<AppLevel> _logger;

    private readonly Compilation _compilation;

    private readonly ImmutableArray<Diagnostic> _diagnostics;

    public RoslynCompilationWrapper(Compilation compilation, ImmutableArray<Diagnostic> diagnostics, IAppLogger<AppLevel> logger)
    {
        _compilation = compilation;
        _logger = logger;
        _diagnostics = diagnostics;
    }

    // context: roslyn, compilation, build
    public ISemanticModelWrapper GetSemanticModel(ISyntaxTreeWrapper wrapper)
    {
        var semanticModel = _compilation.GetSemanticModel((SyntaxTree)wrapper.Tree);
        return new RoslynSemanticModelWrapper(semanticModel, _logger);
    }
}

