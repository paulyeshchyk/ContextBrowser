using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Stategies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: comment, build
public class CSharpCommentTriviaSyntaxParser<TContext> : CSharpCommentSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpCommentTriviaSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
        : base(commentProcessor, onWriteLog)
    {
    }

    // context: comment, build
    public override void Parse(TContext? parent, object node, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (node is not MemberDeclarationSyntax memberDeclaration)
        {
            throw new Exception("node is not MemberDeclarationSyntax");
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Comments, LogLevel.Dbg, $"Parsing comments for {node.GetType().Name}");

        foreach (var trivia in memberDeclaration.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = ExtractComment(trivia);
            _commentAdapter.Process(parent, comment);
        }
    }
}