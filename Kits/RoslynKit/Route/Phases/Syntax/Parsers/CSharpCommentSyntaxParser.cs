using ContextBrowserKit.Log;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.ContextData.Comment;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Syntax.Parsers;

public abstract class CSharpCommentSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly OnWriteLog? _onWriteLog;
    public bool CanParse(object syntax) => syntax is MemberDeclarationSyntax;

    protected CSharpCommentSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, OnWriteLog? onWriteLog)
    {
        _commentAdapter = commentProcessor;
        _onWriteLog = onWriteLog;
    }

    public abstract void Parse(TContext? parent, object node, ISemanticModelWrapper model);

    internal string ExtractComment(SyntaxTrivia trivia)
    {
        return trivia.ToString().TrimStart('/').Trim();
    }
}