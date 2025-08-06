using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Model.Builder;
using RoslynKit.Parser.Code;
using RoslynKit.Parser.Phases;
using RoslynKit.Parser.Syntaxies.Builder;

namespace RoslynKit.Parser.Syntaxies.Parser;

public class TypeSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CommentSyntaxTriviaBuilder<TContext> _triviaCommentBuilder;
    private readonly IContextCollector<TContext> _collector;

    public TypeSyntaxParser(
        IContextCollector<TContext> collector,
        TypeContextInfoBulder<TContext> typeContextInfoBuilder,
        MethodContextInfoBuilder<TContext> methodContextInfoBuilder,
        CommentSyntaxTriviaBuilder<TContext> triviaCommentBuilder,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
        _collector = collector;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is TypeDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not TypeDeclarationSyntax typeSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not TypeDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse type syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);
        var nsName = typeSyntax.GetNamespaceName();
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(typeSyntax, model, nsName, symbol);
        if(typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {nsName}");
            return;
        }

        // Вызовы статических методов
        ClassLevelForeignInstanceScanner.MarkForeignInstanceCalls(model, typeSyntax, _collector);

        _triviaCommentBuilder.ParseComments(typeSyntax, typeContext);

        var methodSyntaxBuilder = new RoslynTypeMethodSyntaxBuilder<TContext>(_options, _onWriteLog, _methodContextInfoBuilder, _triviaCommentBuilder);
        methodSyntaxBuilder.ParseMethodSyntax(typeSyntax, model, nsName, typeContext);
    }
}