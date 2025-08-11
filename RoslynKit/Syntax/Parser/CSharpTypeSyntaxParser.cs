using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

public class CSharpTypeSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypeMethodSyntaxParser<TContext> _methodSyntaxBuilder;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;

    public CSharpTypeSyntaxParser(
        IContextCollector<TContext> collector,
        TypeContextInfoBulder<TContext> typeContextInfoBuilder,
        MethodContextInfoBuilder<TContext> methodContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxBuilder = new CSharpTypeMethodSyntaxParser<TContext>(_methodContextInfoBuilder, _triviaCommentParser, _options, _onWriteLog);
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is ClassDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not ClassDeclarationSyntax typeSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not TypeDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - type syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);
        var nsName = typeSyntax.GetNamespaceName();
        var typeContext = _typeContextInfoBuilder.BuildContextInfoForType(typeSyntax, model, nsName, symbol);
        if(typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {nsName}");
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach(var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(propertySyntax, model);
        }

        _triviaCommentParser.Parse(typeSyntax, typeContext);

        _methodSyntaxBuilder.ParseMethodSyntax(typeSyntax, model, nsName, typeContext);
    }
}
