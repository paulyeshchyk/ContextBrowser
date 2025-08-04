using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Parser.Extractor;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Parser.Phases;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    protected readonly IContextFactory<TContext> _factory;
    private UndefinedInvocationResolver _invocationResolver;

    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, IContextFactory<TContext> factory, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _factory = factory;
        _invocationResolver = new UndefinedInvocationResolver(_semanticInvocationResolver, onWriteLog);
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"parse code: {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _semanticModelBuilder.ModelStorage.GetModel(syntaxTree);
        if(semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SemanticModel not found for {filePath}");
            return;
        }

        // 3. Получаем рут узел
        var root = syntaxTree.GetCompilationUnitRoot(cancellationToken);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        foreach(var method in _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method))
        {
            BuildReferences(method, _collector, options, cancellationToken);
        }
    }

    // context: csharp, read
    public void ParseFile(string filePath, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"read file: {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, options, cancellationToken);
    }

    // context: csharp, read
    protected void BuildReferences(TContext callerContext, IContextCollector<TContext> collector, RoslynCodeParserOptions options, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(callerContext.SymbolName) || !collector.ByFullName.TryGetValue(callerContext.SymbolName, out var callerContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{callerContext.SymbolName}]:Symbol not found in {collector.ByFullName}");
            return;
        }

        var callerSyntaxNode = callerContext.SyntaxNode;
        if(callerSyntaxNode == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{callerContext.SymbolName}]:SyntaxNode is not defined");
            return;
        }

        var invocationList = callerSyntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>().OrderBy(c => c.SpanStart);
        if(!invocationList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"[{callerContext.MethodOwner?.Name ?? string.Empty}.{callerContext.Name}]:No invocation found");
        }

        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(collector, _factory, _onWriteLog, null);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(collector, _onWriteLog, methodContextInfoBuilder);

        foreach(var invocation in invocationList)
        {
            var symbolDto = _invocationResolver.ResolveSymbol(invocation, cancellationToken);

            linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, options);
        }
    }
}

internal static class SymbolExts
{
    public static RoslynCalleeSymbolDto BuildDto(this ISymbol methodSymbol, int spanStart, int spanEnd)
    {
        return new RoslynCalleeSymbolDto() { Name = methodSymbol.GetFullMemberName(), ShortName = methodSymbol.GetShortestName(), SpanStart = spanStart, SpanEnd = spanEnd };
    }
}

internal static class InvocationExts
{
    /// <summary>
    /// Извлекает имя метода и имя владельца (если есть) из InvocationExpressionSyntax,
    /// работая только с синтаксическим деревом.
    /// </summary>
    /// <param name="invocationExpression">Синтаксический узел вызова метода.</param>
    /// <returns>Кортеж с именем метода и именем владельца. Имя владельца может быть null.</returns>
    public static RoslynCalleeSymbolDto GetMethodInfoFromSyntax(this InvocationExpressionSyntax invocationExpression)
    {
        string methodName;
        string ownerName;
        bool isPartial;

        var expression = invocationExpression.Expression;

        if(expression is MemberAccessExpressionSyntax memberAccess)
        {
            isPartial = false;
            methodName = memberAccess.Name.Identifier.Text;
            ownerName = memberAccess.Expression.ToString();
        }
        else if(expression is IdentifierNameSyntax identifierName)
        {
            isPartial = true;
            methodName = identifierName.Identifier.Text;
            ownerName = "__not_parsed__";
        }
        else
        {
            isPartial = true;
            methodName = "__not_parsed__";
            ownerName = "__not_parsed__";
        }

        return new RoslynCalleeSymbolDto() { isPartial = isPartial, Name = $"{ownerName}.{methodName}", ShortName = methodName };
    }
}
