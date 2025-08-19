using ContextBrowserKit.Commandline.Polyfills;
using System.Reflection;

namespace CommandlineKit;

// context: commandline, build
public static class CommandLineParamsValidator
{
    // context: commandline, build
    public static void ValidateRequiredMembers<T>(T options)
    {
        var missingMembers = new List<string>();

        // Находим все свойства, помеченные атрибутом RequiredMemberAttribute
        foreach(var prop in typeof(T).GetProperties())
        {
            if(prop.GetCustomAttribute<System.Runtime.CompilerServices.RequiredMemberAttribute>() != null)
            {
                // Проверяем, что свойство не null
                // Это будет работать для reference-типов
                if(prop.GetValue(options) == null)
                {
                    // Дополнительно можно проверить, является ли тип Value-типом (struct)
                    // Для примера оставим так.

                    // Если свойство не заполнено, ищем его имя аргумента
                    var argAttr = prop.GetCustomAttribute<CommandLineArgumentAttribute>();
                    string argName = argAttr?.Name ?? prop.Name;
                    missingMembers.Add(argName);
                }
            }
        }

        if(missingMembers.Any())
        {
            // Если есть незаполненные обязательные члены, выбрасываем исключение
            throw new ArgumentException($"Следующие обязательные параметры не были указаны: {string.Join(", ", missingMembers)}.");
        }
    }
}