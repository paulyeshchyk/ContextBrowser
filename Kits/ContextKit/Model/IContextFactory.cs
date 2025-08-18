namespace ContextKit.Model;

public interface IContextFactory<T>
{
    T Create(IContextInfo contextInfo);
}
