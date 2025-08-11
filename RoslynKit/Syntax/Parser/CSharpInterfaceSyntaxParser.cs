using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

public class CSharpInterfaceSyntaxParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly InterfaceContextInfoBuilder<TContext> _interfaceContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly MethodContextInfoBuilder<TContext> _methodSyntaxBuilder;

    public CSharpInterfaceSyntaxParser(InterfaceContextInfoBuilder<TContext> interfaceContextInfoBuilder, MethodContextInfoBuilder<TContext> methodSyntaxBuilder, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _interfaceContextInfoBuilder = interfaceContextInfoBuilder;
        _methodSyntaxBuilder = methodSyntaxBuilder;
        _triviaCommentParser = triviaCommentParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 1 - interface syntax");

        var interfaceContext = _interfaceContextInfoBuilder.BuildContextInfoForInterface(interfaceSyntax, model);
        if(interfaceContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for interface.");
            return;
        }

        _triviaCommentParser.Parse(interfaceSyntax, interfaceContext);

        var methodSyntaxes = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>();

        _methodSyntaxBuilder.ParseMethodSyntax(methodSyntaxes, model, interfaceContext.Namespace, interfaceContext);

        // Дополнительно можно добавить парсинг свойств, событий и т.д.
        // var propertySyntaxes = members.OfType<PropertyDeclarationSyntax>();
        // var eventSyntaxes = members.OfType<EventDeclarationSyntax>();
    }
}
