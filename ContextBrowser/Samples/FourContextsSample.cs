namespace ContextBrowser.Samples.FourContextsSample;

// context: create, input
public class InputProcessor
{
    // context: validate, input
    public bool ValidateInput(string raw)
    {
        return !string.IsNullOrWhiteSpace(raw);
    }

    // context: model, input
    public InputModel Parse(string raw)
    {
        return new InputModel { Text = raw };
    }
}

// context: build, transformation
public class Transformer
{
    // context: build, transformation
    public TransformedModel Transform(InputModel input)
    {
        return new TransformedModel { Payload = input.Text.ToUpper() };
    }
}

// context: update, storage
public class StorageWriter
{
    // context: update, storage
    public void Save(TransformedModel model)
    {
        Console.WriteLine($"[STORAGE] Saved: {model.Payload}");
    }
}

// context: share, notification
public class Notifier
{
    // context: share, notification
    public void Notify(string channel, TransformedModel model)
    {
        Console.WriteLine($"[NOTIFY:{channel}] {model.Payload}");
    }
}

// context: create, orchestration
public class FlowOrchestrator
{
    private readonly InputProcessor _input = new();
    private readonly Transformer _transform = new();
    private readonly StorageWriter _storage = new();
    private readonly Notifier _notifier = new();
    public readonly AnotherService Svc = new();

    // context: create, orchestration, share
    public void Run(string raw)
    {
        if(!_input.ValidateInput(raw))
            return;

        var input = _input.Parse(raw);
        var transformed = _transform.Transform(input);

        _storage.Save(transformed);
        _notifier.Notify("default", transformed);

        Svc.DoSomething(transformed);
    }

    //context: create, orchestration, share
    public void CallbackFromAnotherService(TransformedModel model)
    {
        _notifier.Notify("callback", model);
    }
}

// context: share, orchestration
public class AnotherService
{
    private FlowOrchestrator flowOrchestrator = new FlowOrchestrator();

    // context: share, orchestration
    public void DoSomething(TransformedModel model)
    {
        flowOrchestrator.CallbackFromAnotherService(model);
    }
}

// context: model, input
public class InputModel
{
    public string Text { get; set; } = string.Empty;
}

// context: model, transformation
public class TransformedModel
{
    public string Payload { get; set; } = string.Empty;
}