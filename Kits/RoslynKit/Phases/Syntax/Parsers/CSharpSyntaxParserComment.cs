using System.Threading;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public abstract class CSharpSyntaxParserComment<TContext> : ISyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly IContextInfoCommentProcessor<TContext> _commentAdapter;
    protected readonly IAppLogger<AppLevel> _logger;

    public bool CanParseSyntax(object syntax) => syntax is MemberDeclarationSyntax;

    protected CSharpSyntaxParserComment(IContextInfoCommentProcessor<TContext> commentProcessor, IAppLogger<AppLevel> logger)
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