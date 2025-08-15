using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

public class CSharpTypeRecordSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpRecordContextInfoBuilder<TContext> _recordContextInfoBuilder;
    private readonly RoslynCodeParserOptions _options;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;
    private readonly CSharpMethodSyntaxParser<TContext> _methodSyntaxParser;

    public CSharpTypeRecordSyntaxParser(
        IContextCollector<TContext> collector,
        CSharpRecordContextInfoBuilder<TContext> typeContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpMethodSyntaxParser<TContext> methodSyntaxParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        RoslynCodeParserOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _recordContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is RecordDeclarationSyntax;

    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not RecordDeclarationSyntax recordSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not RecordDeclarationSyntax");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - record syntax");

        var recordContext = _recordContextInfoBuilder.BuildContextInfo(parent, recordSyntax, model);
        if(recordContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Parse, LogLevel.Err, $"Syntax \"{recordSyntax}\" was not resolved");
            return;
        }

        var propertySyntaxes = recordSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach(var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(recordContext, propertySyntax, model);
        }

        _triviaCommentParser.Parse(recordContext, recordSyntax, model);

        _methodSyntaxParser.ParseMethodSyntax(recordSyntax, model, recordContext);
    }
}