using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynSyntaxTreeWrapperBuilder<TSyntaxTreeWrapper> : ISyntaxTreeWrapperBuilder<TSyntaxTreeWrapper>
    where TSyntaxTreeWrapper : RoslynSyntaxTreeWrapper
{
    // context: roslyn, build
    public Task<TSyntaxTreeWrapper> BuildAsync(string code, string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: cancellationToken);
        var result = (TSyntaxTreeWrapper)new RoslynSyntaxTreeWrapper(syntaxTree);
        return Task.FromResult(result);
    }
}