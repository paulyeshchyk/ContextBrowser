using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: roslyn, build
public class CSharpDelegateSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpDelegateContextInfoBuilder<TContext> _delegateContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;

    public CSharpDelegateSyntaxParser(CSharpDelegateContextInfoBuilder<TContext> delegateContextInfoBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _delegateContextInfoBuilder = delegateContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    public override bool CanParse(object syntax) => syntax is DelegateDeclarationSyntax;

    // context: roslyn, build
    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not DelegateDeclarationSyntax delegateSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.R_Syntax, LogLevel.Err, $"Syntax is not DelegateDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - delegate syntax");

        var delegateContext = _delegateContextInfoBuilder.BuildContextInfo(default, delegateSyntax, model, cancellationToken);
        if (delegateContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Syntax, LogLevel.Err, $"Syntax \"{delegateSyntax}\" was not resolved");
            return;
        }

        _triviaCommentParser.Parse(delegateContext, delegateSyntax, model, options, cancellationToken);
    }
}
