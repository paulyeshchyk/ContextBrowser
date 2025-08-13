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
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypeMethodSyntaxParser<TContext> _methodSyntaxParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;

    public CSharpTypeSyntaxParser(
        IContextCollector<TContext> collector,
        CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder,
        CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpTypeMethodSyntaxParser<TContext> methodSyntaxParser,
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
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not TypeDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - type syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);

        var typeContext = _typeContextInfoBuilder.BuildContextInfo(parent, syntax, model);
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
