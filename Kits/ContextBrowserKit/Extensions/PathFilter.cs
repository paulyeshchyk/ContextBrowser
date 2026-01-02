using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContextBrowserKit.Extensions;

// context: file, read
public static class PathFilter
{
    // context: file, read
    public static T[] FilteroutPaths<T>(T[] items, string filter, Func<T, string> pathSelector)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return items;
        }

        string[] patterns = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        return items.Where(item => !patterns.Any(pattern => IsMatch(pathSelector(item), pattern))).ToArray();
    }

    // context: file, read
    public static T[] FilterPaths<T>(T[] items, string filter, Func<T, string> pathSelector)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return [];
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