using System.Linq;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders;

public class UmlTransitionDtoBuilder
{
    public const string SUnknownDomainName = "unknown";

    public static UmlTransitionDto? CreateTransition(ContextInfo? caller, ContextInfo? callee, OnWriteLog? log, string? parentUid)
    {
        // Метод -> Метод
        if (caller?.MethodOwner == null)
        {
            log?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"[SKIP] Caller method owner not found for {caller?.Name ?? string.Empty}");
            return default;
        }
        if ((callee?.MethodOwner == null))
        {
            log?.Invoke(AppLevel.P_Tran, LogLevel.Err, $"[SKIP] Callee method owner not found for {callee?.Name ?? string.Empty}");
            return null;
        }

        return CreateTransitionFromMethodCall(caller, callee, log, parentUid);
    }

    private static UmlTransitionDto CreateTransitionFromMethodCall(ContextInfo caller, ContextInfo callee, OnWriteLog? log, string? parentUid)
    {
        var callerInfo = ExtractParticipantInfo(caller);
        var calleeInfo = ExtractParticipantInfo(callee);

        var runContext = callee.MethodOwner?.ClassOwner?.Name?.AlphanumericOnly() ?? string.Empty;//"STATIC";
        var ownerClass = caller.MethodOwner?.ClassOwner?.Name;
        var ownerMethod = caller.MethodOwner?.Name;

        log?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[ADD] method call: {caller.FullName} -> {callee.FullName}");

        return new UmlTransitionDto(
            uid: callee.Identifier,
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
            ownerMethod: ownerMethod);
    }

    private static ParticipantInfo ExtractParticipantInfo(ContextInfo contextInfo)
    {
        if (contextInfo.ClassOwner is ContextInfo classOwner)
        {
            return new ParticipantInfo(classOwner.FullName, classOwner.Name, classOwner.Name, contextInfo.Name);
        }
        if (contextInfo?.MethodOwner?.ClassOwner is ContextInfo methodClassOwner)
        {
            return new ParticipantInfo(methodClassOwner.FullName, methodClassOwner.Name, methodClassOwner.Name, contextInfo.Name);
        }

        // Падение в fallback: или это класс, или что-то нераспарсенное
        return new ParticipantInfo(contextInfo?.FullName, contextInfo?.Name ?? "unknown_context_info_name", contextInfo?.Name ?? "unknown_context_info_name", contextInfo?.Name);
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