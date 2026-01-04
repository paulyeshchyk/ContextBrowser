using System.Threading;
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
    public TSyntaxTreeWrapper Build(string code, string filePath, CancellationToken cancellationToken)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: cancellationToken);
        return (TSyntaxTreeWrapper)new RoslynSyntaxTreeWrapper(syntaxTree);
    }
}