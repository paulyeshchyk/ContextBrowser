namespace LoggerKit.Extensions;

public static class LogerExtensions
{
    public static string BuildFormatterPrefix(object dataToFormat)
    {
        // Получаем строковое представление AppLevel
        string prefix = dataToFormat.ToString() ?? string.Empty;

        // Задаём желаемую длину префикса
        const int prefixLength = 5;

        // Обрезаем или дополняем префикс
        if (prefix.Length > prefixLength)
        {
            // Обрезаем, если префикс длиннее
            prefix = prefix[..prefixLength];
        }
        else if (prefix.Length < prefixLength)
        {
            // Дополняем пробелами, если префикс короче
            // ' ' - символ-заполнитель, prefixLength - целевая длина
            prefix = prefix.PadRight(prefixLength, ' ');
        }

        // Возвращаем отформатированную строку
        return prefix.ToUpper();
    }
}