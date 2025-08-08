namespace ContextBrowser.Samples.Orchestra;

// context: create, test.orchestration
public class FlowOrchestrator
{
    public readonly AnotherService Svc = new();

    // context: create, test.orchestration, share
    public void Run(string raw)
    {
        Svc.DoSomething(string.Empty);
    }

    //context: create, test.orchestration, share
    public void CallbackFromAnotherService(string data)
    {
        Console.WriteLine(string.Empty);
    }
}

// context: share, test.orchestration
public class AnotherService
{
    private FlowOrchestrator flowOrchestrator = new FlowOrchestrator();

    // context: share, test.orchestration
    public void DoSomething(string data)
    {
        Step1(data);
        Step2(data);
    }

    private void Step2(string data)
    {
        flowOrchestrator.CallbackFromAnotherService(data);
    }

    private static void Step1(string data)
    {
        Console.WriteLine(data);
    }
}
