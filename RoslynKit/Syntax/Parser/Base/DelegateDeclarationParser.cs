using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Syntax.Parser.Base;

// context: csharp, build
public class DelegateDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly DelegateContextInfoBuilder<TContext> _delegateContextInfoBuilder;
    private readonly CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;

    public DelegateDeclarationParser(DelegateContextInfoBuilder<TContext> delegateContextInfoBuilder, CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _delegateContextInfoBuilder = delegateContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is DelegateDeclarationSyntax;

    // context: csharp, build
    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if (syntax is not DelegateDeclarationSyntax delegateSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not DelegateDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse delegate syntax: {delegateSyntax.Identifier.Text}");

        var delegateContext = _delegateContextInfoBuilder.BuildContextInfoForDelegate(delegateSyntax, model);
        if (delegateContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{delegateSyntax}\" was not resolved");
            return;
        }

        _triviaCommentBuilder.Parse(delegateSyntax, delegateContext);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Finished parsing DelegateDeclarationSyntax");
    }
}
