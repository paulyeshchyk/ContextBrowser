using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Extensions;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Model.Meta;

public class RoslynSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    private readonly SyntaxTree _syntaxTree;

    public RoslynSyntaxTreeWrapper(SyntaxTree syntaxTree)
    {
        _syntaxTree = syntaxTree;
    }

    public string FilePath => _syntaxTree.FilePath;

    public object Tree => _syntaxTree;

    public Task<object> GetCompilationUnitRootAsync(CancellationToken cancellationToken)
    {
        object result = _syntaxTree.GetCompilationUnitRoot(cancellationToken);
        return Task.FromResult(result);
    }

    public IEnumerable<object> GetAvailableSyntaxies(SemanticOptions options, CancellationToken cancellationToken)
    {
        var root = _syntaxTree.GetCompilationUnitRoot(cancellationToken);
        var result = root.GetMemberDeclarationSyntaxies(options);
        return result;
    }
}
