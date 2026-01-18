using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ContextBrowserKit.Commandline.Polyfills;

namespace CommandlineKit;

// context: commandline, build
public static class CommandLineParamsValidator
{
    // context: commandline, build
    public static void ValidateRequiredMembers<T>(T options)
    {
        var missingMembers = new List<string>();

        // Находим все свойства, помеченные атрибутом RequiredMemberAttribute
        foreach (var prop in typeof(T).GetProperties())
        {
            if (prop.GetCustomAttribute<RequiredMemberAttribute>() == null)
            {
                continue;
            }

            // Проверяем, что свойство не null
            // Это будет работать для reference-типов
            if (prop.GetValue(options) != null)
            {
                continue;
            }

            // Дополнительно можно проверить, является ли тип Value-типом (struct)
            // Для примера оставим так.

            // Если свойство не заполнено, ищем его имя аргумента
            var argAttr = prop.GetCustomAttribute<CommandLineArgumentAttribute>();
            string argName = argAttr?.Name ?? prop.Name;
            missingMembers.Add(argName);
        }

        if (missingMembers.Any())
        {
            // Если есть незаполненные обязательные члены, выбрасываем исключение
            throw new ArgumentException($"Следующие обязательные параметры не были указаны: {string.Join(", ", missingMembers)}.");
        }
    }
}