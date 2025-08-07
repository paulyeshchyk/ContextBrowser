using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Model.Wrappers;
using RoslynKit.Semantic;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Phases;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    protected readonly IContextFactory<TContext> _factory;
    private InvocationSyntaxExtractor _invocationResolver;
    private RoslynCodeParserOptions _options;

    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, IContextFactory<TContext> factory, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _factory = factory;
        _options = options;
        _invocationResolver = new InvocationSyntaxExtractor(_semanticInvocationResolver, _options, onWriteLog);
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing code: {filePath}");

        // 1. Достаём дерево из файла
        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: cancellationToken);

        // 2. Получаем сохранённую модель из хранилища
        var semanticModel = _semanticModelBuilder.ModelStorage.GetModel(syntaxTree);
        if(semanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"SemanticModel not found for {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Generating syntax tree: {filePath}");
        // 3. Получаем рут узел
        var root = syntaxTree.GetCompilationUnitRoot(cancellationToken);

        // 4. Обрабатываем все методы, зарегистрированные в коллекторе
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building method references: {filePath}");
        var theCollection = _collector.GetAll().Where(m => m.ElementType == ContextInfoElementType.method);
        if(!theCollection.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"No method references found in: {filePath}");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.Start);
        foreach(var method in theCollection)
        {
            BuildReferences(method, _collector, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _collector.MergeFakeItems();
    }

    // context: csharp, read
    public void ParseFile(string filePath, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing file: {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, cancellationToken);
    }

    // context: csharp, read
    protected void BuildReferences(TContext callerContext, IContextCollector<TContext> collector, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Build references for {callerContext.Name}");

        var validator = new ReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_onWriteLog);
        var validationResult = validator.Validate(callerContext, collector);

        if(validationResult == null)
        {
            return;
        }

        var callerContextInfo = validationResult.CallerContextInfo;
        var invocationList = validationResult.Invocations;

        var typeContextInfoBuilder = new TypeContextInfoBulder<TContext>(collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(collector, _factory, _onWriteLog);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(collector, _onWriteLog, methodContextInfoBuilder, typeContextInfoBuilder);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Resolving invocations for {callerContext.Name}", LogLevelNode.Start);
        foreach(var invocation in invocationList)
        {
            var symbolDto = _invocationResolver.ResolveSymbol(invocation, cancellationToken);
            if(symbolDto == null)
            {
                continue;
            }
            linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, _options);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
