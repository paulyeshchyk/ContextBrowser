namespace ContextBrowser.Samples;

// context: test.initiator, build
public class Alpha
{
    private Beta beta = new Beta();

    // context: test.initiator, build
    public void Method1()
    {
        beta.Method2();
    }

    // context: test.initiator, build
    public void Method5()
    {
        Console.WriteLine("Alpha Method5"); // финальный вызов
    }
}

// context: test.processor, build
public class Beta
{
    private Gamma gamma = new Gamma();

    // context: test.processor, build
    public void Method2()
    {
        gamma.Method3();
    }
}

// context: test.bridge, build
public class Gamma
{
    private Alpha alpha = new Alpha();

    // context: test.bridge, build
    public void Method3()
    {
        alpha.Method5();
    }
}
