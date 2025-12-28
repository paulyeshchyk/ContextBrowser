using System;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: comment, build
public class CSharpSyntaxParserCommentTrivia<TContext> : CSharpSyntaxParserComment<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpSyntaxParserCommentTrivia(IContextInfoCommentProcessor<TContext> commentProcessor, IAppLogger<AppLevel> logger)
        : base(commentProcessor, logger)
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

        _logger.WriteLog(AppLevel.R_Comments, LogLevel.Dbg, $"Parsing comments for {node.GetType().Name}");

        foreach (var trivia in memberDeclaration.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = ExtractComment(trivia);
            _commentAdapter.Process(parent, comment);
        }
    }
}