using ContextBrowser.DiagramFactory.Builders;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;

namespace ContextBrowser.DiagramFactory.Model;

internal class UmlTransitionDtoBuilder
{
    public const string SUnknownDomainName = "unknown";
    private readonly IControlParticipantResolver _controlResolver;

    public UmlTransitionDtoBuilder(IControlParticipantResolver controlResolver)
    {
        _controlResolver = controlResolver;
    }

    public static UmlTransitionDto? CreateTransition(ContextInfo caller, ContextInfo callee, OnWriteLog? onWriteLog, RoslynCodeParserOptions options)
    {
        if(callee.IsForeignInstance)
        {
            if(!options.ShowForeignInstancies)
            {
                onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"[SKIP] Callee {callee.Name} is foreign instance");
                return null;
            }
        }

        var runContext = callee.MethodOwner?.ClassOwner?.Name;

        var ownerClass = caller.MethodOwner?.ClassOwner?.Name;
        var ownerMethod = caller.MethodOwner?.Name;

        if(string.IsNullOrEmpty(runContext))
        {
            onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"[WARN] RunContext is null for callee: {callee.SymbolName}");
            onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"        MethodOwner: {callee.MethodOwner?.SymbolName}");
            onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"        ClassOwner: {callee.MethodOwner?.ClassOwner?.SymbolName}");
        }

        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Verb, $"Creating UmlTransitionDto: {caller.SymbolName}.{caller.Name} -> {callee.SymbolName}.{callee.Name}");

        return new UmlTransitionDto(
            callerId: caller.SymbolName,
            calleeId: callee.SymbolName,
            callerName: caller.Name,
            calleeName: callee.Name,
            callerClassName: caller.ClassOwner?.Name ?? caller.Name,
            calleeClassName: callee.ClassOwner?.Name ?? callee.Name,
            callerMethod: caller.Name,
            calleeMethod: callee.Name,
            domain: callee.Domains.FirstOrDefault() ?? SUnknownDomainName,
            runContext: runContext,
            ownerClass: ownerClass,
            ownerMethod: ownerMethod
        );
    }
}