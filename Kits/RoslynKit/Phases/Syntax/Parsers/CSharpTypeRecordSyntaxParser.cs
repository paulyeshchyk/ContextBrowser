using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpTypeRecordSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpRecordContextInfoBuilder<TContext> _recordContextInfoBuilder;
    private readonly SemanticOptions _options;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;
    private readonly CSharpMethodSyntaxParser<TContext> _methodSyntaxParser;

    public CSharpTypeRecordSyntaxParser(
        IContextCollector<TContext> collector,
        CSharpRecordContextInfoBuilder<TContext> typeContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpMethodSyntaxParser<TContext> methodSyntaxParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        SemanticOptions options,
        OnWriteLog? onWriteLog) : base(onWriteLog)
    {
        _recordContextInfoBuilder = typeContextInfoBuilder;
        _options = options;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(object syntax) => syntax is RecordDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not RecordDeclarationSyntax recordSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not RecordDeclarationSyntax");
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - record syntax");

        var recordContext = _recordContextInfoBuilder.BuildContextInfo(parent, recordSyntax, model, cancellationToken);
        if (recordContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Syntax, LogLevel.Err, $"Syntax \"{recordSyntax}\" was not resolved");
            return;
        }

        var propertySyntaxes = recordSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(recordContext, propertySyntax, model, options, cancellationToken);
        }

        _triviaCommentParser.Parse(recordContext, recordSyntax, model, options, cancellationToken);

        _methodSyntaxParser.ParseMethodSyntax(recordSyntax, model, recordContext, options, cancellationToken);
    }
}