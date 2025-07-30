namespace ContextBrowser.Samples.Test;

// context: validate, test
public class A
{
    // context: validate, test
    public void Foo()
    {
        var b = new B();
        b.Bar(); // ключевой вызов
    }
}

// context: validate, test
public class B
{
    // context: validate, test
    public void Bar()
    {
    }
}
