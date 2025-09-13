using System;
using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace ContextBrowserKit.Factories;

public interface IContextKeyFactory<TKey>
{
    /// <summary>
    /// Создает новый ключ контекста на основе предоставленных параметров.
    /// </summary>
    TKey Create(string row, string col);
}

public class ContextKeyFactory<TKey> : IContextKeyFactory<TKey>
{
    private readonly Func<string, string, TKey> _creationFunc;

    public ContextKeyFactory(Func<string, string, TKey> creationFunc)
    {
        _creationFunc = creationFunc;
    }

    public TKey Create(string row, string col)
    {
        return _creationFunc(row, col);
    }
}

public interface IContextKeyBuilder
{
    TKey BuildContextKey<TKey>(MatrixOrientationType orientationType, string row, string col, Func<string, string, TKey> createKey);
}
