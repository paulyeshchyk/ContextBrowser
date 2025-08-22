using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Extensions;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Wrappers.Meta;

public class RoslynSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly SyntaxTree _syntaxTree;

    public RoslynSyntaxTreeWrapper(SyntaxTree syntaxTree)
    {
        _syntaxTree = syntaxTree;
    }

    public object GetRoot() => _syntaxTree.GetCompilationUnitRoot();

    public string FilePath => _syntaxTree.FilePath;

    public object Tree => _syntaxTree;

    public object GetCompilationUnitRoot(CancellationToken cancellationToken) => _syntaxTree.GetCompilationUnitRoot(cancellationToken);

    public IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = _syntaxTree.GetCompilationUnitRoot(cancellationToken);
        var result = root.GetMemberDeclarationSyntaxies(options);
        return result;
    }
}
