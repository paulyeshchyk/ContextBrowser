using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Syntax.Parser.ContextInfo;

public abstract class BaseCommentContentInfoBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly OnWriteLog? _onWriteLog;

    protected BaseCommentContentInfoBuilder(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
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