using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Parser.Phases;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog = null;
    protected readonly IContextFactory<TContext> _factory;


    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, IContextFactory<TContext> factory, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _factory = factory;
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

        var invocationList = callerSyntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
        if(!invocationList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[{callerContext.MethodOwner?.Name ?? string.Empty}.{callerContext.Name}]:No invocation found");
        }

        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(collector, _factory, _onWriteLog, null);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(collector, _onWriteLog, methodContextInfoBuilder);

        foreach(var invocation in invocationList)
        {
            var methodSymbol = ResolveSymbol(_semanticInvocationResolver, invocation, cancellationToken);
            if(methodSymbol == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{callerContext.Name}]:Symbol was not resolved for invocation {invocation}");
                continue;
            }

            var calleeSymbolName = methodSymbol.GetFullMemberName();
            var calleeShortestName = methodSymbol.GetShortestName();

            linksInvocationBuilder.LinkInvocation(callerContextInfo, calleeSymbolName, calleeShortestName, options);
        }
    }

    // context: csharp, read
    internal ISymbol? ResolveSymbol(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationSemanticModel = FindSemanticModel(semanticInvocationResolver, byInvocation, byInvocation.SyntaxTree);
        if(invocationSemanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Semantic model was not defined for {byInvocation}");
            return null;
        }

        return GetMethodSymbol(byInvocation, invocationSemanticModel, cancellationToken);
    }

    // context: csharp, read
    internal SemanticModel? FindSemanticModel(ISemanticInvocationResolver semanticInvocationResolver, InvocationExpressionSyntax invocation, SyntaxTree? syntaxTree)
    {
        if(syntaxTree == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Tree was not provided for invocation {invocation}");

            return null;
        }

        return semanticInvocationResolver.Resolve(syntaxTree);
    }

    // context: csharp, read
    internal IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax byInvocation, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // 4. Получаем символ
        var symbolInfo = semanticModel.GetSymbolInfo(byInvocation, cancellationToken);
        if(symbolInfo.Symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"No SymbolInfo was found for {byInvocation}");
            return null;
        }

        if(symbolInfo.Symbol is not IMethodSymbol result)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"SymbolInfo was found for {byInvocation}, but it has no MethodSymbol");
            return null;
        }

        return result;
    }
}
