using ContextKit.Model;

namespace ContextBrowser.DiagramFactory.Builders;

public interface IControlParticipantResolver
{
    bool TryGetControl(ContextInfo caller, out string controlName);
}