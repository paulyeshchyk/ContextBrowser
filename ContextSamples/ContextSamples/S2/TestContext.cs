namespace ContextSamples.ContextSamples.S2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
// context: validate, S2.1, S2.2
public class A
{
    //context: validate, S2.1, S2.2
    public void Foo()
    {
        var b = new B();
        b.Bar(); // ключевой вызов
    }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
//context: validate, S2
public class B
{
    //context: validate, S2, S2.3
    public void Bar()
    {
    }
}