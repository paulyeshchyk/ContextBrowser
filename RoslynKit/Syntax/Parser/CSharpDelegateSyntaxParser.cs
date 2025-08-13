using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

// context: csharp, build
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

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is DelegateDeclarationSyntax;

    // context: csharp, build
    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not DelegateDeclarationSyntax delegateSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not DelegateDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 1 - delegate syntax");

        var delegateContext = _delegateContextInfoBuilder.BuildContextInfo(default, delegateSyntax, model);
        if(delegateContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{delegateSyntax}\" was not resolved");
            return;
        }

        _triviaCommentParser.Parse(delegateContext, delegateSyntax, model);
    }
}
