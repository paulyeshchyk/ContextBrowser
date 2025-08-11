using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using UmlKit.Extensions;
using UmlKit.Renderer.Model;

namespace UmlKit.Renderer.Builder;

public class UmlTransitionDtoBuilder
{
    public const string SUnknownDomainName = "unknown";

    public static UmlTransitionDto? CreateTransition(ContextInfo? caller, ContextInfo? callee, OnWriteLog? log, Guid? parentUid)
    {
        // Метод -> Метод
        if(!(caller?.MethodOwner != null && callee?.MethodOwner != null))
        {
            log?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"[SKIP] Transition unsupported for {caller?.Name ?? string.Empty} -> {callee?.Name ?? string.Empty}");
            return null;
        }

        return CreateTransitionFromMethodCall(caller, callee, log, parentUid);
    }

    private static UmlTransitionDto CreateTransitionFromMethodCall(ContextInfo caller, ContextInfo callee, OnWriteLog? log, Guid? parentUid)
    {
        var callerInfo = ExtractParticipantInfo(caller);
        var calleeInfo = ExtractParticipantInfo(callee);

        var runContext = callee.MethodOwner?.ClassOwner?.Name?.AlphanumericOnly();
        var ownerClass = caller.MethodOwner?.ClassOwner?.Name;
        var ownerMethod = caller.MethodOwner?.Name;

        log?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[ADD] method call: {caller.GetDebugSymbolName()} -> {callee.GetDebugSymbolName()}");

        return new UmlTransitionDto(
            uid: callee.Uid,
            parentUid: parentUid,
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
        return new ParticipantInfo(contextInfo?.SymbolName, contextInfo?.Name ?? "unknown_context_info_name", contextInfo?.Name ?? "unknown_context_info_name", contextInfo?.Name);
    }
}

internal record struct ParticipantInfo(string? Id, string Name, string ClassName, string? MethodName)
{
    public static implicit operator (string? Id, string Name, string ClassName, string? MethodName)(ParticipantInfo value)
    {
        return (value.Id, value.Name, value.ClassName, value.MethodName);
    }

    public static implicit operator ParticipantInfo((string Id, string Name, string ClassName, string? MethodName) value)
    {
        return new ParticipantInfo(value.Id, value.Name, value.ClassName, value.MethodName);
    }
}