using System;

namespace ContextSamples.ContextSamples;

// context: S6, build
public enum TheType
{
    alpha,
    beta,
    gamma
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
// context: S6, build
public class Alpha
{
    internal readonly Beta beta = new Beta();

    internal readonly bool Buildable = true;

    // context: S6, model
    public TheType ItemType => TheType.alpha;

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

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
// context: S6.1, build
public class Beta
{
    private readonly Gamma gamma = new Gamma();

    // context: S6.1, model
    public TheType ItemType => TheType.beta;

    // context: S6.1, build
    public void Method2()
    {
        gamma.Method3();
    }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
// context: S6.2, build
public class Gamma
{
    private readonly Alpha alpha = new Alpha();

    // context: S6.2, model
    public TheType ItemType => TheType.gamma;

    // context: S6.2, build
    public void Method3()
    {
        alpha.Method5();
    }
}
