namespace UmlKit.Infrastructure.Options.Activation;

public record DiagramTransitionOptions : IActivationObserver
{
    private readonly bool _useCall;
    private readonly bool _useDone;

    public bool UseCall { get; private set; }

    public bool UseDone { get; set; }

    public DiagramTransitionOptions(bool useCall, bool useDone)
    {
        _useCall = useCall;
        UseCall = useCall;

        _useDone = useDone;
        UseDone = useDone;
    }

    public void Update(bool useActivation)
    {
        UseCall = useActivation ? _useCall : false;
        UseDone = useActivation ? _useDone : false;
    }
}
