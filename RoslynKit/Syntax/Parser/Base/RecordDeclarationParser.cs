using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Syntax.Parser.Base;

public class RecordDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly RecordContextInfoBuilder<TContext> _recordContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;
    private readonly PropertyDeclarationParser<TContext> _propertyDeclarationParser;

    public RecordDeclarationParser(
        IContextCollector<TContext> collector,
        RecordContextInfoBuilder<TContext> typeContextInfoBuilder,
        PropertyDeclarationParser<TContext> propertyDeclarationParser,
        CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _recordContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _triviaCommentBuilder = triviaCommentBuilder;
        _propertyDeclarationParser = propertyDeclarationParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is RecordDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if (syntax is not RecordDeclarationSyntax typeSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not RecordDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Dbg, $"Parse type syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);
        var nsName = typeSyntax.GetNamespaceName();
        var typeContext = _recordContextInfoBuilder.BuildContextInfoForRecord(typeSyntax, model, nsName, symbol);
        if (typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {nsName}");
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(propertySyntax, model);
        }

        _triviaCommentBuilder.Parse(typeSyntax, typeContext);
    }
}