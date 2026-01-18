using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Assembly;
using RoslynKit.Syntax;
using RoslynKit.Wrappers;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserTypeClass<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private const string SErrorNotClassDeclarationSyntax = "Syntax ({0}) is not ClassDeclarationSyntax";
    private const string SErrorNullTypeContext = "Syntax \"{0}\" was not resolved in {1}";
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly CSharpSyntaxParserMethod<TContext> _methodSyntaxParser;
    private readonly CSharpSyntaxParserTypeProperty<TContext> _propertyDeclarationParser;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;
    private readonly IRoslynSymbolLoader<MemberDeclarationSyntax, ISymbol> _symbolLoader;

    public CSharpSyntaxParserTypeClass(
        CSharpSyntaxParserTypeProperty<TContext> propertyDeclarationParser,
        CSharpSyntaxParserMethod<TContext> methodSyntaxParser,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        IRoslynSymbolLoader<MemberDeclarationSyntax, ISymbol> symbolLoader) : base(logger)
    {
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _symbolLoader = symbolLoader;
    }

    public override bool CanParseSyntax(object syntax) => syntax is ClassDeclarationSyntax;

    // context: syntax, build
    public override async Task ParseAsync(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not ClassDeclarationSyntax typeSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, string.Format(SErrorNotClassDeclarationSyntax, nameof(syntax)));
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - type syntax");

        _ = await _symbolLoader.LoadSymbolAsync(typeSyntax, model, cancellationToken).ConfigureAwait(false);

        var typeContext = await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(parent, typeSyntax, model, cancellationToken).ConfigureAwait(false);
        if (typeContext == null)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, string.Format(SErrorNullTypeContext, typeSyntax, typeSyntax.GetNamespaceOrGlobal()));
            return;
        }

        var propertySyntaxes = typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            await _propertyDeclarationParser.ParseAsync(typeContext, propertySyntax, model, options, cancellationToken).ConfigureAwait(false);
        }

        await _triviaCommentParser.ParseAsync(typeContext, typeSyntax, model, options, cancellationToken).ConfigureAwait(false);

        await _methodSyntaxParser.ParseMethodSyntaxAsync(typeSyntax, model, typeContext, options, cancellationToken).ConfigureAwait(false);
    }
}
