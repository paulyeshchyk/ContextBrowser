using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;

namespace RoslynKit.Tree;

public class RoslynSyntaxTreeWrapperBuilder : ISyntaxTreeWrapperBuilder
{
    public ISyntaxTreeWrapper Build(string code, string filePath, CancellationToken cancellationToken)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: cancellationToken);
        return new RoslynSyntaxTreeWrapper(syntaxTree);
    }
}