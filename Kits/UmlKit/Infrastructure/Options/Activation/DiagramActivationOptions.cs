using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UmlKit.Infrastructure.Options.Activation;

public record DiagramActivationOptions
{
    private bool _useActivation;
    private readonly bool _useActivationCallRaw;
    private readonly List<IActivationObserver> _observers = new();

    public bool UseActivation
    {
        get => _useActivation;
        set
        {
            if (_useActivation != value)
            {
                _useActivation = value;
                NotifyObservers();
            }
        }
    }

    public bool UseActivationCall => UseActivation ? _useActivationCallRaw : false;

    [JsonConstructor]
    public DiagramActivationOptions(bool useActivation, bool useActivationCall)
    {
        _useActivation = useActivation;
        _useActivationCallRaw = useActivationCall;
    }

    public void RegisterObserver(IActivationObserver observer)
    {
        _observers.Add(observer);
        observer.Update(_useActivation);
    }

    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.Update(_useActivation);
        }
    }
}
