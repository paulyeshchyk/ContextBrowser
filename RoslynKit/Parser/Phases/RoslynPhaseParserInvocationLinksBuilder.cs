using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using RoslynKit.Model;
using RoslynKit.Parser.Extractor;

namespace RoslynKit.Parser.Phases;

// context: csharp, build
public class RoslynPhaseParserInvocationLinksBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;

    public RoslynPhaseParserInvocationLinksBuilder(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, MethodContextInfoBuilder<TContext> methodContextInfoBuilder)
    {
        _collector = collector;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
    }

    // context: csharp, update
    public TContext? LinkInvocation(TContext callerContextInfo, RoslynCalleeSymbolDto symbolDto, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building invocation [{callerContextInfo.Name}]", LogLevelNode.Start);

        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if(calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{symbolDto.Name}]");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Adding invocation Reference [{calleeContextInfo.Name}]");
        callerContextInfo.References.Add(calleeContextInfo);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Adding invocation InvokedBy [{callerContextInfo.Name}]");
        calleeContextInfo.InvokedBy.Add(callerContextInfo);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return calleeContextInfo;
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, RoslynCalleeSymbolDto symbolDto, RoslynCodeParserOptions options)
    {
        if(symbolDto.Symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[FAIL] Symbol is null in RoslynCalleeSymbolDto for {symbolDto.Name}");
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Symbol is passed into FindOrCreateCalleeNode: {symbolDto.Symbol.ToDisplayString()}");
        }

        // 1. Поиск по symbolDto.Name
        if(_collector.BySymbolDisplayName.TryGetValue(symbolDto.Name, out var calleeContextInfo))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Found ContextInfo by SymbolName: {symbolDto.Name}");
            return calleeContextInfo;
        }

        // 2. Альтернатива: поиск по полной сигнатуре через IMethodSymbol
        if(symbolDto.Symbol is IMethodSymbol methodSymbol)
        {
            var fullName = methodSymbol.ToDisplayString();
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FALLBACK] Trying full signature: {fullName}");

            if(_collector.BySymbolDisplayName.TryGetValue(fullName, out var fallbackCallee))
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[HIT] Recovered callee via full symbol: {fullName}");
                return fallbackCallee;
            }
            else
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[MISS] Symbol exists but fallback lookup failed: {fullName}");
            }
        }

        // 3. Фолбэк — создание искусственного узла
        if(!options.CreateFailedCallees)
        {
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[CREATE] Fallback fake callee created for: {symbolDto.Name}");

        var methodmodel = new MethodSyntaxModel
        {
            methodName = symbolDto.ShortName,
            methodFullName = symbolDto.Name,
            spanStart = symbolDto.SpanStart,
            spanEnd = symbolDto.SpanEnd
        };

        return _methodContextInfoBuilder.BuildContextInfoForMethod(methodmodel, "FakeNamespace", callerContextInfo, null);
    }
}

public record struct RoslynCalleeSymbolDto
{
    public bool isPartial { get; init; }

    public string Name { get; init; }

    public string ShortName { get; init; }

    public int SpanStart { get; init; }

    public int SpanEnd { get; init; }

    public ISymbol? Symbol { get; set; }
}
