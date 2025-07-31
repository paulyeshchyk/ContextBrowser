using System.Reflection;

namespace ContextBrowser.Infrastructure.CommandLine;

public class CommandLineParser
{
    private const string SPrefixValueError = "Ошибка: Аргумент '{0}' должен начинаться с '{1}'.";

    public bool TryParse<T>(string[] args, out T? options, out string? errorMessage)
    {
        options = default;

        if(FindHelpParameter<T>(args, out errorMessage))
        {
            return false;
        }

        errorMessage = null;

        try
        {
            options = Parse<T>(args);
            return true;
        }
        catch(Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    internal bool FindHelpParameter<T>(string[] args, out string? errorMessage)
    {
        errorMessage = null;
        var helpPattern = $"{CommandLineDefaults.SArgumentPrefix}{CommandLineDefaults.SHelpKeyword}";
        var helpIndex = Array.FindIndex(args, a => a.Equals(helpPattern, StringComparison.OrdinalIgnoreCase));

        // Если --help найден
        if(helpIndex == -1)
        {
            return false;
        }

        var specificParamName = FindParameterNameForHelp(args, helpIndex);

        errorMessage = CommandLineHelpProducer.GenerateHelpText<T>(CommandLineDefaults.SArgumentPrefix, specificParamName);
        return true;
    }

    private static string? FindParameterNameForHelp(string[] args, int helpIndex)
    {
        // Проверяем, есть ли следующий аргумент после "--help"
        return (helpIndex + 1 < args.Length)
            ? args[helpIndex + 1]
            : null;
    }

    internal T Parse<T>(string[] args)
    {
        if(args.Any(a => a.Equals($"{CommandLineDefaults.SArgumentPrefix}{CommandLineDefaults.SHelpKeyword}", StringComparison.OrdinalIgnoreCase)))
        {
            // Пропускаем все аргументы, так как --help должен быть обработан TryParse
            return Activator.CreateInstance<T>();
        }


        T options = Activator.CreateInstance<T>();

        // Проходим по всем аргументам
        for(int i = 0; i < args.Length; i += 2)
        {
            string argName = args[i];
            if(!argName.StartsWith(CommandLineDefaults.SArgumentPrefix))
            {
                throw new ArgumentException(string.Format(SPrefixValueError, argName, CommandLineDefaults.SArgumentPrefix));
            }
            string argValue = (i + 1 < args.Length) ? args[i + 1] : string.Empty;

            // Разделяем имя аргумента по точке, чтобы получить путь к свойству
            string[] propertyPath = argName.Substring(2).Split('.');

            // Используем новый вспомогательный метод для установки значения
            CommandLineNodeIterator.SetNestedPropertyValue(options, propertyPath, argValue);
        }

        CommandLineParamsValidator.ValidateRequiredMembers(options);

        return options;
    }
}

internal static class CommandLineNodeIterator
{
    private const string SParsingErrorUnknownPropertyMessage = "Ошибка: Неизвестное свойство '{0}' в пути '{1}'.";
    private const string SParsingErrorCantInstanciateObjectMessage = "Ошибка: Не удалось создать экземпляр для типа '{0}'.";


    public static void SetNestedPropertyValue<T>(T targetObject, string[] propertyPath, string value)
    {
        object? currentObject = targetObject;
        PropertyInfo? currentProperty = null;

        for(int i = 0; i < propertyPath.Length; i++)
        {
            // Находим свойство по имени
            currentProperty = currentObject?.GetType().GetProperty(
                propertyPath[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if(currentProperty == null)
            {
                throw new ArgumentException(string.Format(SParsingErrorUnknownPropertyMessage, propertyPath[i], string.Join(".", propertyPath)));
            }

            // Если это не последнее свойство в пути, создаём вложенный объект
            if(i < propertyPath.Length - 1)
            {
                object? nestedObject = currentProperty.GetValue(currentObject);
                if(nestedObject == null)
                {
                    // Если вложенный объект null, создаём его
                    if(currentProperty.PropertyType.IsClass)
                    {
                        nestedObject = Activator.CreateInstance(currentProperty.PropertyType);
                        currentProperty.SetValue(currentObject, nestedObject);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SParsingErrorCantInstanciateObjectMessage, currentProperty.PropertyType.Name));
                    }
                }
                currentObject = nestedObject;
            }
            else
            {
                // Если это последнее свойство, устанавливаем значение
                object convertedValue = CommandLineArgumentValueConverter.ConvertValue(currentProperty.PropertyType, value);
                currentProperty.SetValue(currentObject, convertedValue);
            }
        }
    }
}