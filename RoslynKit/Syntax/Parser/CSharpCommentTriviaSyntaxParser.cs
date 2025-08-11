using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.Comment;

namespace RoslynKit.Syntax.Parser;

public class CSharpCommentTriviaSyntaxParser<TContext> : CSharpCommentSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public CSharpCommentTriviaSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
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