using System;
using System.Linq;
using CommandlineKit.Model;

namespace CommandlineKit;

// context: commandline, build
public class CommandLineParser
{
    private const string SPrefixValueError = "Ошибка: Аргумент '{0}' должен начинаться с '{1}'.";

    // context: commandline, build
    public bool TryParse<T>(string[] args, out T? options, out string? errorMessage)
    {
        options = default;

        if (FindHelpParameter<T>(args, out errorMessage))
        {
            return false;
        }

        errorMessage = null;

        try
        {
            options = Parse<T>(args);
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    // context: commandline, read
    internal bool FindHelpParameter<T>(string[] args, out string? errorMessage)
    {
        errorMessage = null;
        var helpPattern = $"{CommandLineDefaults.SArgumentPrefix}{CommandLineDefaults.SHelpKeyword}";
        var helpIndex = Array.FindIndex(args, a => a.Equals(helpPattern, StringComparison.OrdinalIgnoreCase));

        // Если --help найден
        if (helpIndex == -1)
        {
            return false;
        }

        var specificParamName = FindParameterNameForHelp(args, helpIndex);

        errorMessage = CommandLineHelpProducer.GenerateHelpText<T>(CommandLineDefaults.SArgumentPrefix, specificParamName);
        return true;
    }

    // context: commandline, read
    internal static string? FindParameterNameForHelp(string[] args, int helpIndex)
    {
        // Проверяем, есть ли следующий аргумент после "--help"
        return (helpIndex + 1 < args.Length)
            ? args[helpIndex + 1]
            : null;
    }

    // context: commandline, build
    internal T Parse<T>(string[] args)
    {
        if (args.Any(a => a.Equals($"{CommandLineDefaults.SArgumentPrefix}{CommandLineDefaults.SHelpKeyword}", StringComparison.OrdinalIgnoreCase)))
        {
            // Пропускаем все аргументы, так как --help должен быть обработан TryParse
            return Activator.CreateInstance<T>();
        }

        T options = Activator.CreateInstance<T>();

        // Проходим по всем аргументам
        for (int i = 0; i < args.Length; i += 2)
        {
            string argName = args[i];
            if (!argName.StartsWith(CommandLineDefaults.SArgumentPrefix))
            {
                throw new ArgumentException(string.Format(SPrefixValueError, argName, CommandLineDefaults.SArgumentPrefix));
            }
            string argValue = (i + 1 < args.Length) ? args[i + 1] : string.Empty;

            // Разделяем имя аргумента по точке, чтобы получить путь к свойству
            string[] propertyPath = argName[2..].Split('.');

            // Используем новый вспомогательный метод для установки значения
            CommandLineNodeIterator.SetNestedPropertyValue(options, propertyPath, argValue);
        }

        CommandLineParamsValidator.ValidateRequiredMembers(options);

        return options;
    }
}
