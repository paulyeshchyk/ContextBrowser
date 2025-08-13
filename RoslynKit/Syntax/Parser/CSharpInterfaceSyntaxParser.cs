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
    private readonly CSharpInterfaceContextInfoBuilder<TContext> _interfaceContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpTypeMethodSyntaxParser<TContext> _methodSyntaxParser;

    public CSharpInterfaceSyntaxParser(CSharpInterfaceContextInfoBuilder<TContext> interfaceContextInfoBuilder, CSharpTypeMethodSyntaxParser<TContext> methodSyntaxParser, CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _interfaceContextInfoBuilder = interfaceContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _methodSyntaxParser = methodSyntaxParser;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 1 - interface syntax");

        //TODO: do check parent
        var interfaceContext = _interfaceContextInfoBuilder.BuildContextInfo(default, interfaceSyntax, model);
        if(interfaceContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for interface.");
            return;
        }

        _triviaCommentParser.Parse(interfaceContext, interfaceSyntax, model);

        var methodSyntaxes = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>();

        _methodSyntaxParser.ParseMethodSyntax(interfaceContext, methodSyntaxes, model);

        // Дополнительно можно добавить парсинг свойств, событий и т.д.
        // var propertySyntaxes = members.OfType<PropertyDeclarationSyntax>();
        // var eventSyntaxes = members.OfType<EventDeclarationSyntax>();
    }
}
