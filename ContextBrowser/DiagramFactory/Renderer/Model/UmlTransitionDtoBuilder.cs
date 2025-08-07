using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Extensions;

namespace ContextBrowser.DiagramFactory.Renderer.Model;

internal class UmlTransitionDtoBuilder
{
    public const string SUnknownDomainName = "unknown";

    public static UmlTransitionDto? CreateTransition(ContextInfo? caller, ContextInfo? callee, OnWriteLog? log, RoslynCodeParserOptions options)
    {
        // Если callee помечен как foreign, и их нельзя показывать — игнорируем
        if((callee?.IsForeignInstance ?? false) && !options.ShowForeignInstancies)
        {
            log?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"[SKIP] Callee {callee.Name} is foreign instance");
            return null;
        }

        // Метод -> Метод
        if(caller?.MethodOwner != null && callee?.MethodOwner != null)
            return CreateTransitionFromMethodCall(caller, callee, log);

        // По другим типам можно расширить тут логику

        log?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"[SKIP] Transition unsupported for: {caller?.Name ?? string.Empty} -> {callee?.Name ?? string.Empty}");
        return null;
    }

    private static UmlTransitionDto CreateTransitionFromMethodCall(ContextInfo caller, ContextInfo callee, OnWriteLog? log)
    {
        var callerInfo = ExtractParticipantInfo(caller);
        var calleeInfo = ExtractParticipantInfo(callee);

        var runContext = callee.MethodOwner?.ClassOwner?.Name.AlphanumericOnly();
        var ownerClass = caller.MethodOwner?.ClassOwner?.Name;
        var ownerMethod = caller.MethodOwner?.Name;

        log?.Invoke(AppLevel.P_Bld, LogLevel.Verb, $"Creating UmlTransitionDto: {caller.SymbolName}.{caller.Name} -> {callee.SymbolName}.{callee.Name}");

        return new UmlTransitionDto(
            callerId: callerInfo.Id,
            calleeId: calleeInfo.Id,
            callerName: callerInfo.Name,
            calleeName: calleeInfo.Name,
            callerClassName: callerInfo.ClassName,
            calleeClassName: calleeInfo.ClassName,
            callerMethod: callerInfo.MethodName,
            calleeMethod: calleeInfo.MethodName,
            domain: callee.Domains.FirstOrDefault() ?? "unknown",
            runContext: runContext,
            ownerClass: ownerClass,
            ownerMethod: ownerMethod
        );
    }

    private static ParticipantInfo ExtractParticipantInfo(ContextInfo contextInfo)
    {
        if(contextInfo.ClassOwner is ContextInfo classOwner)
        {
            return new ParticipantInfo(classOwner.SymbolName, classOwner.Name, classOwner.Name, contextInfo.Name);
        }
        if(contextInfo?.MethodOwner?.ClassOwner is ContextInfo methodClassOwner)
        {
            return new ParticipantInfo(methodClassOwner.SymbolName, methodClassOwner.Name, methodClassOwner.Name, contextInfo.Name);
        }

        // Падение в fallback: или это класс, или что-то нераспарсенное
        return new ParticipantInfo(contextInfo?.SymbolName, contextInfo?.Name, contextInfo?.Name, contextInfo?.Name);
    }
}

internal record struct ParticipantInfo(string? Id, string? Name, string? ClassName, string? MethodName)
{
    public static implicit operator (string? Id, string? Name, string? ClassName, string? MethodName)(ParticipantInfo value)
    {
        return (value.Id, value.Name, value.ClassName, value.MethodName);
    }

    public static implicit operator ParticipantInfo((string Id, string? Name, string? ClassName, string? MethodName) value)
    {
        return new ParticipantInfo(value.Id, value.Name, value.ClassName, value.MethodName);
    }
}