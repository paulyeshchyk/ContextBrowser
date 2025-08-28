using System.Text.RegularExpressions;

namespace ContextBrowserKit.Extensions;

public static class PathFilter
{
    public static T[] FilteroutPaths<T>(T[] items, string filter, Func<T, string> pathSelector)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return items;
        }

        string[] patterns = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        return items.Where(item => !patterns.Any(pattern => IsMatch(pathSelector(item), pattern))).ToArray();
    }

    // Аналогичный метод для включения путей
    public static T[] FilterPaths<T>(T[] items, string filter, Func<T, string> pathSelector)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return Array.Empty<T>();
        }

        string[] patterns = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        return items.Where(item => patterns.Any(pattern => IsMatch(pathSelector(item), pattern))).ToArray();
    }

    private static bool IsMatch(string filePath, string pattern)
    {
        // нормализация слэшей
        filePath = filePath.Replace("\\", "/");
        pattern = pattern.Replace("\\", "/");

        // общий перевод glob → regex
        string regexPattern =
            "^" +
            Regex.Escape(pattern)
                .Replace(@"\*\*", ".*")    // ** = любая последовательность (в т.ч. /)
                .Replace(@"\*", @"[^\/]*")  // * = любые символы кроме /
            + "$";

        var result = Regex.IsMatch(filePath, regexPattern, RegexOptions.IgnoreCase);
        return result;
    }
}