using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ContextBrowserKit.Commandline.Polyfills;

namespace CommandlineKit;

// context: commandline, build
public static class CommandLineHelpProducer
{
    private const string SParamNotFoundTemplate = "Ошибка: Параметр '{0}' не найден.";

    // context: commandline, build
    public static string GenerateHelpText<T>(string argumentPrefix)
    {
        var helpText = new StringBuilder();
        helpText.AppendLine("Доступные параметры:");

        // Итерируемся по свойствам модели и ищем атрибуты
        foreach (var prop in typeof(T).GetProperties())
        {
            var argAttr = prop.GetCustomAttribute<CommandLineArgumentAttribute>();
            if (argAttr != null)
            {
                // Помечаем обязательные параметры
                var isRequired = prop.GetCustomAttribute<RequiredMemberAttribute>() != null;
                var requiredMarker = isRequired ? " (обязательный)" : string.Empty;

                helpText.AppendLine($"  {argumentPrefix}{argAttr.Name}{requiredMarker}: {argAttr.Description}");
            }
        }
        return helpText.ToString();
    }

    // context: commandline, build
    public static string GenerateHelpText<T>(string argumentPrefix, string? specificParamName)
    {
        var helpText = new StringBuilder();

        var allProperties = typeof(T).GetProperties();

        // Если запрошена помощь по конкретному параметру
        if (!string.IsNullOrWhiteSpace(specificParamName))
        {
            // Ищем свойство по имени параметра
            var targetProp = allProperties.FirstOrDefault(p =>
                p.GetCustomAttribute<CommandLineArgumentAttribute>()?.Name.Equals(specificParamName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (targetProp != null)
            {
                var argAttr = targetProp.GetCustomAttribute<CommandLineArgumentAttribute>();
                if (argAttr != null)
                {
                    var isRequired = targetProp.GetCustomAttribute<RequiredMemberAttribute>() != null;
                    var requiredMarker = isRequired ? " (обязательный)" : string.Empty;

                    helpText.AppendLine($"Справка по параметру '{argumentPrefix}{argAttr.Name}':");
                    helpText.AppendLine($"  {argAttr.Description}{requiredMarker}");
                    return helpText.ToString();
                }
                else
                {
                    helpText.AppendLine(string.Format(SParamNotFoundTemplate, specificParamName));
                    helpText.AppendLine();
                }
            }
            else
            {
                helpText.AppendLine(string.Format(SParamNotFoundTemplate, specificParamName));
                helpText.AppendLine();
            }
        }

        // Показываем полный текст справки (по умолчанию или после ошибки)
        helpText.AppendLine("Доступные параметры:");

        foreach (var prop in allProperties)
        {
            var argAttr = prop.GetCustomAttribute<CommandLineArgumentAttribute>();
            if (argAttr == null)
            {
                continue;
            }

            var isRequired = prop.GetCustomAttribute<RequiredMemberAttribute>() != null;
            var requiredMarker = isRequired ? " (обязательный)" : string.Empty;

            helpText.AppendLine($"  {argumentPrefix}{argAttr.Name}{requiredMarker}: {argAttr.Description}");
        }
        return helpText.ToString();
    }
}