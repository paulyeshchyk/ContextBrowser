using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Route.Wrappers.Meta;
using SemanticKit.Model;

namespace RoslynKit.Route.Tree;

public class RoslynSyntaxTreeWrapperBuilder : ISyntaxTreeWrapperBuilder
{
    public ISyntaxTreeWrapper BuildTree(string code, string filePath, CancellationToken cancellationToken)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(text: code, path: filePath, cancellationToken: cancellationToken);
        return new RoslynSyntaxTreeWrapper(syntaxTree);
    }
}