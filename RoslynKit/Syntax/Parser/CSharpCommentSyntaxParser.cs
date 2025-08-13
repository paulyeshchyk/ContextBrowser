using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.Comment;

namespace RoslynKit.Syntax.Parser;

public abstract class CSharpCommentSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly OnWriteLog? _onWriteLog;
    public bool CanParse(MemberDeclarationSyntax syntax) => true;

    protected CSharpCommentSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
    {
        _commentAdapter = commentProcessor;
        _onWriteLog = onWriteLog;
    }

    public abstract void Parse(TContext? parent, MemberDeclarationSyntax node, SemanticModel model);

    internal string ExtractComment(SyntaxTrivia trivia)
    {
        return trivia.ToString().TrimStart('/').Trim();
    }
}