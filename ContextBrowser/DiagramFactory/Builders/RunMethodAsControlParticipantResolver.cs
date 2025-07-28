using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

public class RunMethodAsControlParticipantResolver : IControlParticipantResolver
{
    public bool TryGetControl(ContextInfo caller, out string controlName)
    {
        controlName = default!;
        if(caller.MethodOwner?.Name == "Run" && caller.ClassOwner?.Name is { } owner)
        {
            controlName = owner;
            return true;
        }

        return false;
    }
}
