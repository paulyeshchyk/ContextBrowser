using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Model.Builder;
using RoslynKit.Parser.Code;
using RoslynKit.Parser.Phases;

namespace RoslynKit.Parser.Syntaxies.Builder;

public class RoslynTypeMethodSyntaxBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private RoslynCodeParserOptions _options;
    private OnWriteLog? _onWriteLog;
    private MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private CommentSyntaxTriviaBuilder<TContext> _triviaCommentBuilder;

    public RoslynTypeMethodSyntaxBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, MethodContextInfoBuilder<TContext> methodContextInfoBuilder, CommentSyntaxTriviaBuilder<TContext> triviaCommentBuilder)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
    }

    //context: csharp, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, SemanticModel semanticModel, string nsName, TContext typeContext)
    {
        if(availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(_options);
        if(!methodDeclarationSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = _methodContextInfoBuilder.BuildContextInfoForMethods(semanticModel, nsName, typeContext, methodDeclarationSyntaxies);

        foreach(var item in buildItems)
        {
            _triviaCommentBuilder.ParseComments(item.Item2, item.Item1);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
