using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Syntax.Parser.Base;

public class MethodDeclarationParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private RoslynCodeParserOptions _options;
    private OnWriteLog? _onWriteLog;
    private MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;

    public MethodDeclarationParser(MethodContextInfoBuilder<TContext> methodContextInfoBuilder, CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
    }

    //context: csharp, build
    public void ParseMethodSyntax(MemberDeclarationSyntax availableSyntax, SemanticModel semanticModel, string nsName, TContext typeContext)
    {
        if (availableSyntax is not TypeDeclarationSyntax typeSyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{typeContext.Name}]:Syntax is not TypeDeclaration syntax");

            return;
        }

        var methodDeclarationSyntaxies = typeSyntax.FilteredMethodsList(_options);
        if (!methodDeclarationSyntaxies.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{typeContext.Name}]:Syntax has no methods in List");
            return;
        }

        var buildItems = _methodContextInfoBuilder.BuildContextInfoForMethods(semanticModel, nsName, typeContext, methodDeclarationSyntaxies);

        foreach (var item in buildItems)
        {
            _triviaCommentBuilder.Parse(item.Item2, item.Item1);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}

internal static class TypeDeclarationDyntaxExts
{
    public static IEnumerable<MethodDeclarationSyntax> FilteredMethodsList(this TypeDeclarationSyntax typeSyntax, RoslynCodeParserOptions options)
    {
        return typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Where(methodDeclarationSyntax =>
            {
                var mod = methodDeclarationSyntax.GetModifierType();
                return mod.HasValue && (options.MethodModifierTypes?.Contains(mod.Value) ?? false);
            })
            .OrderBy(m => m.SpanStart);
    }
}
