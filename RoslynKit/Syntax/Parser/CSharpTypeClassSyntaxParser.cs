using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;
using RoslynKit.Syntax.AssemblyLoader;

namespace RoslynKit.Syntax.Parser;

public class CSharpTypeClassSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpMethodSyntaxParser<TContext> _methodSyntaxParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;

    public CSharpTypeClassSyntaxParser(
        IContextCollector<TContext> collector,
        CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpMethodSyntaxParser<TContext> methodSyntaxParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is ClassDeclarationSyntax;

    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not ClassDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax ({nameof(syntax)}) is not ClassDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - type syntax");

        var symbol = CSharpSymbolLoader.LoadSymbol(typeSyntax, model, _onWriteLog, CancellationToken.None);

        var typeContext = _typeContextInfoBuilder.BuildContextInfo(parent, typeSyntax, model);
        if(typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {typeSyntax.GetNamespaceName()}");
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach(var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(typeContext, propertySyntax, model);
        }

        _triviaCommentParser.Parse(typeContext, typeSyntax, model);

        _methodSyntaxParser.ParseMethodSyntax(typeSyntax, model, typeContext);
    }
}
