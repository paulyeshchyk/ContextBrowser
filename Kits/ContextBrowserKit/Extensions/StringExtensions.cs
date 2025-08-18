using System.Text.RegularExpressions;

namespace ContextBrowserKit.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Возвращает подстроку до первой точки или всю строку, если точка не найдена.
    /// </summary>
    public static string BeforeFirstDot(this string input)
    {
        if(input == null)
            return string.Empty;
        var match = Regex.Match(input, @"^[^\.]+");
        return match.Success ? match.Value : input;
    }

    /// <summary>
    /// Заменяет все символы, кроме латинских букв, цифр и кириллицы, на подчёркивание.
    /// </summary>
    public static string ToAlphanumericUnderscore(this string input)
    {
        if(input == null)
            return string.Empty;
        return Regex.Replace(input, @"[^\p{L}\p{Nd}]", "_");
    }

    public static string AlphanumericOnly(this string s, string replaceBy = "_") => Regex.Replace(s, "[^a-zA-Z0-9]", replaceBy);
}