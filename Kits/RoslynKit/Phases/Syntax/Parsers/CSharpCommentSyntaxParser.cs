using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Stategies;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public abstract class CSharpCommentSyntaxParser<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly IAppLogger<AppLevel> _logger;

    public bool CanParse(object syntax) => syntax is MemberDeclarationSyntax;

    protected CSharpCommentSyntaxParser(IContextInfoCommentProcessor<TContext> commentProcessor, IAppLogger<AppLevel> logger)
    {
        _commentAdapter = commentProcessor;
        _logger = logger;
    }

    public abstract void Parse(TContext? parent, object node, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken);

    internal string ExtractComment(SyntaxTrivia trivia)
    {
        return trivia.ToString().TrimStart('/').Trim();
    }
}