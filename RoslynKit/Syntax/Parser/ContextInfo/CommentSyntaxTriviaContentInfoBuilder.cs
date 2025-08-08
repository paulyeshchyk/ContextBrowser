using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.Parser.ContextInfo;

public class CommentSyntaxTriviaContentInfoBuilder<TContext> : BaseCommentContentInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CommentSyntaxTriviaContentInfoBuilder(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
        : base(commentProcessor, onWriteLog)
    {
    }

    public override void Parse(MemberDeclarationSyntax node, TContext context)
    {
        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Parsing comments for {node.GetType().Name}");

        foreach(var trivia in node.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
        {
            string comment = ExtractComment(trivia);
            _commentAdapter.Process(comment, context);
        }
    }
}