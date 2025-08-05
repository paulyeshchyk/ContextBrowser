namespace ContextBrowser.Samples;

// context: initiator, build
public class Alpha
{
    private Beta beta = new Beta();

    // context: initiator, build
    public void Method1()
    {
        beta.Method2();
    }

    // context: initiator, build
    public void Method5()
    {
        Console.WriteLine("Alpha Method5"); // финальный вызов
    }
}

// context: processor, build
public class Beta
{
    private Gamma gamma = new Gamma();

    // context: processor, build
    public void Method2()
    {
        gamma.Method3();
    }
}

// context: bridge, build
public class Gamma
{
    private Alpha alpha = new Alpha();

    // context: bridge, build
    public void Method3()
    {
        alpha.Method5();
    }
}
