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

public class CSharpRecordSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly RecordContextInfoBuilder<TContext> _recordContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;

    public CSharpRecordSyntaxParser(
        IContextCollector<TContext> collector,
        RecordContextInfoBuilder<TContext> typeContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _recordContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is RecordDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not RecordDeclarationSyntax typeSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not RecordDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - record syntax");

        var symbol = model.GetDeclaredSymbol(typeSyntax);
        var nsName = typeSyntax.GetNamespaceName();
        var typeContext = _recordContextInfoBuilder.BuildContextInfoForRecord(typeSyntax, model, nsName, symbol);
        if(typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Syntax \"{typeSyntax}\" was not resolved in {nsName}");
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach(var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(propertySyntax, model);
        }

        _triviaCommentParser.Parse(typeSyntax, typeContext);
    }
}