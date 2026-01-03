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
public class CSharpSyntaxParserInterface<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly CSharpSyntaxParserTypeProperty<TContext> _propertyDeclarationParser;
    private readonly CSharpSyntaxParserMethod<TContext> _methodSyntaxParser;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;


    public CSharpSyntaxParserInterface(
        CSharpSyntaxParserTypeProperty<TContext> propertyDeclarationParser,
        CSharpSyntaxParserMethod<TContext> methodSyntaxParser,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
        : base(logger)
    {
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    public override bool CanParseSyntax(object syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, "Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1 - interface syntax");

        var interfaceContext = _contextInfoBuilderDispatcher.DispatchAndBuild(default, interfaceSyntax, model, cancellationToken);
        if (interfaceContext == null)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, "Failed to build context for interface.");
            return;
        }

        var propertySyntaxes = interfaceSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(interfaceContext, propertySyntax, model, options, cancellationToken);
        }

        _triviaCommentParser.Parse(interfaceContext, interfaceSyntax, model, options, cancellationToken);

        var methodSyntaxes = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>();

        _methodSyntaxParser.ParseMethodSyntax(interfaceContext, methodSyntaxes, model, cancellationToken);
    }
}
