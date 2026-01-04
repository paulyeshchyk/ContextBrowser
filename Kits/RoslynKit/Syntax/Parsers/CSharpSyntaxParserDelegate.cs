using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserDelegate<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public CSharpSyntaxParserDelegate(
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
        : base(logger)
    {
        _triviaCommentParser = triviaCommentParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    public override bool CanParseSyntax(object syntax) => syntax is DelegateDeclarationSyntax;

    // context: roslyn, build
    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not DelegateDeclarationSyntax delegateSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, "Syntax is not DelegateDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - delegate syntax");

        var delegateContext = _contextInfoBuilderDispatcher.DispatchAndBuild(default, delegateSyntax, model, cancellationToken);
        if (delegateContext == null)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"Syntax \"{delegateSyntax}\" was not resolved");
            return;
        }

        _triviaCommentParser.Parse(delegateContext, delegateSyntax, model, options, cancellationToken);
    }
}
