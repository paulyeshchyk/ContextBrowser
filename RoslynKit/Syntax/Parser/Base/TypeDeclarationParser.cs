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

public class TypeDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;
    private readonly MethodDeclarationParser<TContext> _methodSyntaxBuilder;
    private readonly PropertyDeclarationParser<TContext> _propertyDeclarationParser;

    public TypeDeclarationParser(
        IContextCollector<TContext> collector,
        TypeContextInfoBulder<TContext> typeContextInfoBuilder,
        MethodContextInfoBuilder<TContext> methodContextInfoBuilder,
        PropertyDeclarationParser<TContext> propertyDeclarationParser,
        CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxBuilder = new MethodDeclarationParser<TContext>(_methodContextInfoBuilder, _triviaCommentBuilder, _options, _onWriteLog);
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is TypeDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if (syntax is not TypeDeclarationSyntax typeSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not TypeDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse type syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);
        var nsName = typeSyntax.GetNamespaceName();
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(typeSyntax, model, nsName, symbol);
        if (typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {nsName}");
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(propertySyntax, model);
        }

        _triviaCommentBuilder.Parse(typeSyntax, typeContext);

        _methodSyntaxBuilder.ParseMethodSyntax(typeSyntax, model, nsName, typeContext);
    }
}