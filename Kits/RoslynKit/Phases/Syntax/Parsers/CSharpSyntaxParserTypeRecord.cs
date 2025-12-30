using System;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpSyntaxParserTypeRecord<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpRecordContextInfoBuilder<TContext> _recordContextInfoBuilder;
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly CSharpSyntaxParserTypeProperty<TContext> _propertyDeclarationParser;
    private readonly CSharpSyntaxParserMethod<TContext> _methodSyntaxParser;

    public CSharpSyntaxParserTypeRecord(
        IContextCollector<TContext> collector,
        CSharpRecordContextInfoBuilder<TContext> typeContextInfoBuilder,
        CSharpSyntaxParserTypeProperty<TContext> propertyDeclarationParser,
        CSharpSyntaxParserMethod<TContext> methodSyntaxParser,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        IAppLogger<AppLevel> logger) : base(logger)
    {
        _recordContextInfoBuilder = typeContextInfoBuilder;
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

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - record syntax");

        var recordContext = _recordContextInfoBuilder.BuildContextInfo(parent, recordSyntax, model, cancellationToken);
        if (recordContext == null)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"Syntax \"{recordSyntax}\" was not resolved");
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