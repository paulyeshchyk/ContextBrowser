using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options;

namespace ContextBrowser.Services;

// context: settings, read
public class AppSettingsStore : IAppOptionsStore
{
    private AppOptions? _options = null;

    // context: settings, read
    public AppOptions Options() { return _options!; }

    // context: settings, update
    public void SetOptions(object options)
    {
        _options = (options as AppOptions) ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Возвращает конкретный объект опций по его типу, используя рефлексию.
    /// </summary>
    /// <typeparam name="T">Тип запрашиваемого объекта опций.</typeparam>
    /// <param name="recursive">Если true, будет выполнен рекурсивный поиск по всему графу объектов.</param>
    /// <returns>Экземпляр объекта опций.</returns>
    /// <exception cref="InvalidOperationException">Вызывается, если опции не установлены.</exception>
    /// <exception cref="ArgumentException">Вызывается, если свойство типа T не найдено.</exception>
    public T GetOptions<T>(bool recursive = true) where T : class
    {
        if (_options == null)
        {
            throw new InvalidOperationException("AppOptions have not been set.");
        }

        if (!recursive)
        {
            var property = typeof(AppOptions)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.PropertyType == typeof(T));

            if (property == null)
            {
                throw new ArgumentException($"Property of type {typeof(T).Name} not found in AppOptions.");
            }

            return (T)property.GetValue(_options)!;
        }
        else
        {
            var foundObjects = new List<T>();
            var visited = new HashSet<object>();

            FindRecursive(_options, foundObjects, visited);

            if (foundObjects.Count == 0)
            {
                throw new ArgumentException($"No object of type {typeof(T).Name} found in AppOptions graph.");
            }

            if (foundObjects.Count > 1)
            {
                throw new InvalidOperationException($"Multiple objects of type {typeof(T).Name} found. Please specify which one you need.");
            }

            return foundObjects.First();
        }
    }

    private void FindRecursive<T>(object currentObject, List<T> foundObjects, HashSet<object> visited) where T : class
    {
        if (currentObject == null || visited.Contains(currentObject))
        {
            return;
        }

        visited.Add(currentObject);

        var type = currentObject.GetType();

        if (type.IsPrimitive || (type.Namespace?.StartsWith("System") ?? false))
        {
            return;
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = property.GetValue(currentObject);
            if (value == null)
                continue;

            if (property.PropertyType == typeof(T))
            {
                foundObjects.Add((T)value);
            }

            FindRecursive(value, foundObjects, visited);
        }
    }
}
