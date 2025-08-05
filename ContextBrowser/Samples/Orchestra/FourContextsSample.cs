namespace ContextBrowser.Samples.Orchestra;

// context: create, orchestration
public class FlowOrchestrator
{
    public readonly AnotherService Svc = new();

    // context: create, orchestration, share
    public void Run(string raw)
    {
        Svc.DoSomething(string.Empty);
    }

    //context: create, orchestration, share
    public void CallbackFromAnotherService(string data)
    {
        Console.WriteLine(string.Empty);
    }
}

// context: share, orchestration
public class AnotherService
{
    private FlowOrchestrator flowOrchestrator = new FlowOrchestrator();

    // context: share, orchestration
    public void DoSomething(string data)
    {
        flowOrchestrator.CallbackFromAnotherService(data);
    }
}
