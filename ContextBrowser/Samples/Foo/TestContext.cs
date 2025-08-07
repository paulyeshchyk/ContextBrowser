namespace ContextBrowser.Samples.Foo;

// context: validate, test.Foo
public class A
{
    // context: validate, test.Foo
    public void Foo()
    {
        var b = new B();
        b.Bar(); // ключевой вызов
    }
}

// context: validate, test.Foo
public class B
{
    // context: validate, test.Foo
    public void Bar()
    {
    }
}