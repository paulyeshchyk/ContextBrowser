namespace ContextBrowser.Samples.Orchestra;

// context: create, S3
public class FlowOrchestrator
{
    public readonly AnotherService Svc = new();

    // context: create, S3, share
    public void Run(string raw)
    {
        Svc.DoSomething(string.Empty);
    }

    //context: create, S3, share
    public void CallbackFromAnotherService(string data)
    {
        Console.WriteLine(string.Empty);
    }
}

// context: share, S3
public class AnotherService
{
    private FlowOrchestrator flowOrchestrator = new FlowOrchestrator();

    // context: share, S3
    public void DoSomething(string data)
    {
        Step1(data);
        Step2(data);
    }

    // context: share, S3
    public static void Step1(string data)
    {
        Console.WriteLine(data);
    }

    // context: share, S3
    public void Step2(string data)
    {
        flowOrchestrator.CallbackFromAnotherService(data);
    }
}
