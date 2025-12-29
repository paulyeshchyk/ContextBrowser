namespace ContextSamples.ContextSamples.S2;

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

//context: validate, S2
public class B
{
    //context: validate, S2, S2.3
    public void Bar()
    {
    }
}