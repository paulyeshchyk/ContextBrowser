using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders.ContextDiagramBuilders.Model;

internal class UmlTransitionDtoBuilder
{
    public const string SUnknownDomainName = "unknown";
    private readonly IControlParticipantResolver _controlResolver;

    public UmlTransitionDtoBuilder(IControlParticipantResolver controlResolver)
    {
        _controlResolver = controlResolver;
    }

    public static UmlTransitionDto CreateTransition(ContextInfo caller, ContextInfo callee)
    {
        var runContext = callee.MethodOwner?.ClassOwner?.Name;
        if(string.IsNullOrEmpty(runContext))
        {
            Console.WriteLine($"[WARN] RunContext is null for callee: {callee.SymbolName}");
            Console.WriteLine($"        MethodOwner: {callee.MethodOwner?.SymbolName}");
            Console.WriteLine($"        ClassOwner: {callee.MethodOwner?.ClassOwner?.SymbolName}");
        }

        return new UmlTransitionDto
        {
            CallerId = caller.SymbolName,
            CalleeId = callee.SymbolName,
            CallerName = caller.Name,
            CalleeName = callee.Name,
            CallerClassName = caller.ClassOwner?.Name ?? caller.Name,
            CalleeClassName = callee.ClassOwner?.Name ?? callee.Name,
            CallerMethod = caller.Name,
            CalleeMethod = callee.Name,
            Domain = callee.Domains.FirstOrDefault() ?? SUnknownDomainName,
            RunContext = runContext
        };
    }
}