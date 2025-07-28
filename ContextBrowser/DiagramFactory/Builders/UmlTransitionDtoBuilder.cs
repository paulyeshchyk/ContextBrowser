using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

internal class UmlTransitionDtoBuilder
{
    private readonly IControlParticipantResolver _controlResolver;

    public UmlTransitionDtoBuilder(IControlParticipantResolver controlResolver)
    {
        _controlResolver = controlResolver;
    }

    public static UmlTransitionDto CreateTransition(ContextInfo caller, ContextInfo callee)
    {
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
            Domain = callee.Domains.FirstOrDefault() ?? "unknown",
            RunContext = callee.MethodOwner?.ClassOwner?.Name
        };
    }
}
