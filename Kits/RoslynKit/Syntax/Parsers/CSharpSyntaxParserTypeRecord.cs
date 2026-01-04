using System;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserTypeRecord<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly CSharpSyntaxParserTypeProperty<TContext> _propertyDeclarationParser;
    private readonly CSharpSyntaxParserMethod<TContext> _methodSyntaxParser;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public CSharpSyntaxParserTypeRecord(
        CSharpSyntaxParserTypeProperty<TContext> propertyDeclarationParser,
        CSharpSyntaxParserMethod<TContext> methodSyntaxParser,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger) : base(logger)
    {
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    public override bool CanParseSyntax(object syntax) => syntax is RecordDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not RecordDeclarationSyntax recordSyntax)
        {
            throw new ArgumentException($"Syntax ({nameof(syntax)}) is not RecordDeclarationSyntax");
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - record syntax");

        var recordContext = _contextInfoBuilderDispatcher.DispatchAndBuild(parent, recordSyntax, model, cancellationToken);
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