using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.Comment;

namespace RoslynKit.Syntax.Parser;

public abstract class CSharpCommentSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly OnWriteLog? _onWriteLog;

    protected CSharpCommentSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
    {
        _commentAdapter = commentProcessor;
        _onWriteLog = onWriteLog;
    }

    public abstract void Parse(MemberDeclarationSyntax node, TContext context);

    internal string ExtractComment(SyntaxTrivia trivia)
    {
        return trivia.ToString().TrimStart('/').Trim();
    }
}