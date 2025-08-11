using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Model.ModelBuilder;
using RoslynKit.Model.Resolver;
using RoslynKit.Model.Wrappers;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Phases;

// context: csharp, read
public class RoslynPhaseParserReferenceResolver<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private ISemanticModelBuilder _semanticModelBuilder;
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    private IContextFactory<TContext> _factory;
    private RoslynCodeParserOptions _options;
    private InvocationReferenceBuilder<TContext> _invocationReferenceBuilder;

    public RoslynPhaseParserReferenceResolver(IContextCollector<TContext> collector, IContextFactory<TContext> factory, ISemanticModelBuilder modelBuilder, ISemanticInvocationResolver semanticInvocationResolver, RoslynCodeParserOptions options, OnWriteLog? onWriteLog = null) : base()
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _factory = factory;
        _options = options;
        _invocationReferenceBuilder = new InvocationReferenceBuilder<TContext>(_onWriteLog, _factory, _semanticInvocationResolver, _options, _collector);
    }

    // context: csharp, read
    public void ParseCode(string code, string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing code: phase 2 - {filePath}");

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

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building invocatons", LogLevelNode.Start);
        foreach(var method in theCollection)
        {
            _invocationReferenceBuilder.BuildReferences(method, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        _collector.MergeFakeItems();
    }

    public void ParseFiles(string[] filePaths, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 2", LogLevelNode.Start);

        foreach(var file in filePaths)
        {
            ParseFile(file, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    // context: csharp, read
    public void ParseFile(string filePath, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 2 - {filePath}");

        var code = File.ReadAllText(filePath);
        ParseCode(code, filePath, cancellationToken);
    }
}

public class InvocationReferenceBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private OnWriteLog? _onWriteLog;
    protected readonly IContextFactory<TContext> _factory;
    private InvocationSyntaxExtractor _invocationSyntaxExtractor;
    private RoslynCodeParserOptions _options;
    private IContextCollector<TContext> _collector;
    private ISemanticInvocationResolver _semanticInvocationResolver;

    public InvocationReferenceBuilder(OnWriteLog? onWriteLog, IContextFactory<TContext> factory, ISemanticInvocationResolver semanticInvocationResolver, RoslynCodeParserOptions options, IContextCollector<TContext> collector)
    {
        _semanticInvocationResolver = semanticInvocationResolver;
        _onWriteLog = onWriteLog;
        _factory = factory;
        _options = options;
        _collector = collector;
        _invocationSyntaxExtractor = new InvocationSyntaxExtractor(_semanticInvocationResolver, _options, onWriteLog);
    }

    // context: csharp, read
    public void BuildReferences(TContext callerContext, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Build references for [{callerContext.GetDebugName()}]");

        var validator = new ReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_onWriteLog);

        var validationResult = validator.Validate(callerContext, _collector);
        if(validationResult == null)
        {
            return;
        }

        BuildInvocations(callerContext, validationResult, cancellationToken);
    }

    private void BuildInvocations(TContext callerContext, ReferenceBuilderValidator<TContext, InvocationExpressionSyntax>.ValidationResult validationResult, CancellationToken cancellationToken)
    {
        var callerContextInfo = validationResult.CallerContextInfo;
        var invocationList = validationResult.Invocations;

        var typeContextInfoBuilder = new TypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new MethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(_collector, _onWriteLog, methodContextInfoBuilder, typeContextInfoBuilder);

        var invocationLinker = new InvocationLinker<TContext>(linksInvocationBuilder, _onWriteLog, _invocationSyntaxExtractor, _options);
        invocationLinker.Link(invocationList, callerContext, callerContextInfo, cancellationToken);
    }
}

public class InvocationLinker<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private RoslynPhaseParserInvocationLinksBuilder<TContext> _linksInvocationBuilder;
    private OnWriteLog? _onWriteLog;
    private InvocationSyntaxExtractor _invocationSyntaxExtractor;
    private RoslynCodeParserOptions _options;

    public InvocationLinker(RoslynPhaseParserInvocationLinksBuilder<TContext> linksInvocationBuilder, OnWriteLog? onWriteLog, InvocationSyntaxExtractor invocationSyntaxExtractor, RoslynCodeParserOptions options)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _onWriteLog = onWriteLog;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
        _options = options;
    }

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Resolving invocations for [{callerContext.GetDebugSymbolName()}]", LogLevelNode.Start);
        foreach(var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, RoslynPhaseParserInvocationLinksBuilder<TContext> linksInvocationBuilder, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveSymbol(invocation, cancellationToken);
        if(symbolDto == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL]: Symbol was not found [{invocation}]");
            return;
        }

        linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, _options);
    }
}
