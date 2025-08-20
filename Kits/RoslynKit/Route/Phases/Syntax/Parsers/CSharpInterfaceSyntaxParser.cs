using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Route.Phases.ContextInfoBuilder;
using SemanticKit.Model;

namespace RoslynKit.Route.Phases.Syntax.Parsers;

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
        OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _interfaceContextInfoBuilder = interfaceContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _propertyDeclarationParser = propertyDeclarationParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(object syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
        if (syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - interface syntax");

        var interfaceContext = _interfaceContextInfoBuilder.BuildContextInfo(default, interfaceSyntax, model, cancellationToken);
        if (interfaceContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for interface.");
            return;
        }

        var propertySyntaxes = interfaceSyntax.Members.OfType<PropertyDeclarationSyntax>();
        foreach (var propertySyntax in propertySyntaxes)
        {
            _propertyDeclarationParser.Parse(interfaceContext, propertySyntax, model, cancellationToken);
        }

        _triviaCommentParser.Parse(interfaceContext, interfaceSyntax, model, cancellationToken);

        var methodSyntaxes = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>();

        _methodSyntaxParser.ParseMethodSyntax(interfaceContext, methodSyntaxes, model, cancellationToken);
    }
}
