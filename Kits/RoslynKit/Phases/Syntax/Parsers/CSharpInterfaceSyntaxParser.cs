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

public class CSharpInterfaceSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpInterfaceContextInfoBuilder<TContext> _interfaceContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypePropertyParser<TContext> _propertyDeclarationParser;
    private readonly CSharpMethodSyntaxParser<TContext> _methodSyntaxParser;

    public CSharpInterfaceSyntaxParser(
        CSharpInterfaceContextInfoBuilder<TContext> interfaceContextInfoBuilder,
        CSharpTypePropertyParser<TContext> propertyDeclarationParser,
        CSharpMethodSyntaxParser<TContext> methodSyntaxParser,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        IAppLogger<AppLevel> logger)
        : base(logger)
    {
        _interfaceContextInfoBuilder = interfaceContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(object syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - interface syntax");

        var interfaceContext = _interfaceContextInfoBuilder.BuildContextInfo(default, interfaceSyntax, model, cancellationToken);
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
