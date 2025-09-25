using System;
using System.Collections.Generic;

namespace ContextKit.Model.Collector;

public static class ContextInfoFullNameIndexBuilder
{
    public static Dictionary<string, TContext> Build<TContext>(IEnumerable<TContext> items)
        where TContext : IContextWithReferences<TContext>
    {
        var resultDictionary = new Dictionary<string, TContext>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            // Проверяем, что FullName не пустой, как и в вашем коде
            if (string.IsNullOrWhiteSpace(item.FullName))
            {
                continue;
            }

            // Здесь мы пытаемся добавить элемент. 
            // Метод TryAdd безопасен и не выбросит исключение, если ключ уже есть.
            resultDictionary.TryAdd(item.FullName, item);
        }

        return resultDictionary;
    }
}