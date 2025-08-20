using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.ContextData.Comment;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Syntax.Parsers;

public class CSharpCommentTriviaSyntaxParser<TContext> : CSharpCommentSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpCommentTriviaSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
        : base(commentProcessor, onWriteLog)
    {
    }

    public override void Parse(TContext? parent, object node, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        if (node is not MemberDeclarationSyntax memberDeclaration)
        {
            throw new Exception("node is not MemberDeclarationSyntax");
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Parsing comments for {node.GetType().Name}");

        foreach (var trivia in memberDeclaration.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = ExtractComment(trivia);
            _commentAdapter.Process(parent, comment);
        }
    }
}