namespace ContextBrowser.Samples.Foo;

// context: validate, S2
public class A
{
    // context: validate, S2
    public void Foo()
    {
        var b = new B();
        b.Bar(); // ключевой вызов
    }
}

// context: validate, S2
public class B
{
    // context: validate, S2
    public void Bar()
    {
    }
}