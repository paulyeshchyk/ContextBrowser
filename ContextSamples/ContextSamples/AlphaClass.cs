namespace ContextBrowser.Samples;

// context: S6, build
public class Alpha
{
    private Beta beta = new Beta();

    // context: S6, build
    public void Method1()
    {
        beta.Method2();
    }

    // context: S6, build
    public void Method5()
    {
        Console.WriteLine("Alpha Method5"); // финальный вызов
    }
}

// context: S6.1, build
public class Beta
{
    private Gamma gamma = new Gamma();

    // context: S6.1, build
    public void Method2()
    {
        gamma.Method3();
    }
}

// context: S6.2, build
public class Gamma
{
    private Alpha alpha = new Alpha();

    // context: S6.2, build
    public void Method3()
    {
        alpha.Method5();
    }
}
