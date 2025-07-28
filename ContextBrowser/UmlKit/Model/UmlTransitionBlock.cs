namespace ContextBrowser.UmlKit.Model;

public class UmlTransitionBlock
{
    public UmlTransitionBlock(IUmlDeclarable subject, UmlActivate? activate = null, UmlDeactivate? deactivate = null)
    {
        Subject = subject;
        Activate = activate;
        Deactivate = deactivate;
        IsActivated = false;
    }

    public IUmlDeclarable Subject { get; }

    public UmlActivate? Activate { get; set; }

    public UmlDeactivate? Deactivate { get; set; }

    public bool IsActivated { get; private set; }

    public void MarkActivated()
    {
        IsActivated = true;
    }

    public void MarkDeactivated()
    {
        IsActivated = false;
    }
}