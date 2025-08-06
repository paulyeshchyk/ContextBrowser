using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Syntax.Parser.Base;

public class InterfaceDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly InterfaceContextInfoBuilder<TContext> _interfaceContextInfoBuilder;
    private readonly CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;
    private readonly MethodContextInfoBuilder<TContext> _methodSyntaxBuilder;

    public InterfaceDeclarationParser(InterfaceContextInfoBuilder<TContext> interfaceContextInfoBuilder, MethodContextInfoBuilder<TContext> methodSyntaxBuilder, CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder, OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _interfaceContextInfoBuilder = interfaceContextInfoBuilder;
        _methodSyntaxBuilder = methodSyntaxBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is InterfaceDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if (syntax is not InterfaceDeclarationSyntax interfaceSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not InterfaceDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse interface syntax: {interfaceSyntax.Identifier.Text}", LogLevelNode.Start);

        var interfaceContext = _interfaceContextInfoBuilder.BuildContextInfoForInterface(interfaceSyntax, model);
        if (interfaceContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for interface.", LogLevelNode.End);
            return;
        }

        _triviaCommentBuilder.Parse(interfaceSyntax, interfaceContext);

        var methodSyntaxes = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>();

        _methodSyntaxBuilder.ParseMethodSyntax(methodSyntaxes, model, interfaceContext.Namespace, interfaceContext);

        // Дополнительно можно добавить парсинг свойств, событий и т.д.
        // var propertySyntaxes = members.OfType<PropertyDeclarationSyntax>();
        // var eventSyntaxes = members.OfType<EventDeclarationSyntax>();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Finished parsing InterfaceDeclarationSyntax", LogLevelNode.End);
    }
}
