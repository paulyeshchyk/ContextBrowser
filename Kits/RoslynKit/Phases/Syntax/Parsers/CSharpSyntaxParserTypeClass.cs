using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Wrappers;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

public class CSharpSyntaxParserTypeClass<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private const string SErrorNotClassDeclarationSyntax = "Syntax ({0}) is not ClassDeclarationSyntax";
    private const string SErrorNullTypeContext = "Syntax \"{0}\" was not resolved in {1}";
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly CSharpSyntaxParserMethod<TContext> _methodSyntaxParser;
    private readonly CSharpSyntaxParserTypeProperty<TContext> _propertyDeclarationParser;

    public CSharpSyntaxParserTypeClass(
        IContextCollector<TContext> collector,
        CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder,
        CSharpSyntaxParserTypeProperty<TContext> propertyDeclarationParser,
        CSharpSyntaxParserMethod<TContext> methodSyntaxParser,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        IAppLogger<AppLevel> logger) : base(logger)
    {
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(object syntax) => syntax is ClassDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not ClassDeclarationSyntax typeSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, string.Format(SErrorNotClassDeclarationSyntax, nameof(syntax)));
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - type syntax");

        var symbol = CSharpSymbolLoader.LoadSymbol(typeSyntax, model, _logger, cancellationToken);

        var typeContext = _typeContextInfoBuilder.BuildContextInfo(parent, typeSyntax, model, cancellationToken);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, string.Format(SErrorNullTypeContext, typeSyntax, typeSyntax.GetNamespaceOrGlobal()));
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(typeContext, propertySyntax, model, options, cancellationToken);
        }

        _triviaCommentParser.Parse(typeContext, typeSyntax, model, options, cancellationToken);

        _methodSyntaxParser.ParseMethodSyntax(typeSyntax, model, typeContext, options, cancellationToken);
    }
}
