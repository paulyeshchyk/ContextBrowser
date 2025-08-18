namespace CommandlineKit;

// context: commandline, build
public static class CommandLineArgumentValueConverter
{
    // context: commandline, build
    public static object ConvertValue(Type targetType, string value)
    {
        if(targetType == typeof(IEnumerable<string>))
        {
            // Обрабатываем IEnumerable<string>
            return value.Split(',').Select(s => s.Trim());
        }

        if(targetType.IsEnum)
        {
            // Обрабатываем Enum
            return Enum.Parse(targetType, value, true);
        }

        // Используем базовый конвертер для остальных типов
        return Convert.ChangeType(value, targetType);
    }
}