using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContextBrowserKit.Extensions;

public static partial class StringExtensions
{
    public static string BeforeDot(this string input, int byGroupId = 1, int groupsCountToJoin = 1)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var parts = input.Split('.');
        if (byGroupId <= 0 || groupsCountToJoin <= 0)
            return string.Empty;

        // индекс в массиве (byGroupId начинается с 1, поэтому -1)
        int startIndex = byGroupId - 1;
        if (startIndex >= parts.Length)
            return string.Join(".", parts); // вернуть всю строку

        // ограничим количество элементов
        int count = Math.Min(groupsCountToJoin, parts.Length - startIndex);

        return string.Join(".", parts.Skip(startIndex).Take(count));
    }

    /// <summary>
    /// Заменяет все символы, кроме латинских букв, цифр и кириллицы, на подчёркивание.
    /// </summary>
    public static string ToAlphanumericUnderscore(this string input)
    {
        if (input == null)
            return string.Empty;
        return AlphanumericUnderscoreRegex().Replace(input, "_");
    }

    public static string AlphanumericOnly(this string s, string replaceBy = "_") => AlphanumericOnlyRegex().Replace(s, replaceBy);

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex AlphanumericOnlyRegex();

    [GeneratedRegex(@"[^\p{L}\p{Nd}]")]
    private static partial Regex AlphanumericUnderscoreRegex();
}