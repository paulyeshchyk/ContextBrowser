﻿using System.Text.RegularExpressions;

namespace ContextBrowser.ContextKit.Parser.CommentParser;

public interface ICommentParsingStrategy<T>
{
    void Execute(string comment, T container);
}

public static class CommentWithKeywordParser
{
    /// <summary>
    /// Извлекает контент из строки, начинающейся с ключевого слова и двоеточия.
    /// </summary>
    /// <param name="keyword">Ключевое слово для поиска.</param>
    /// <param name="comment">Строка, которую нужно распарсить.</param>
    /// <returns>Содержимое после двоеточия, если найдено; в противном случае null.</returns>
    public static string? ExtractContent(string keyword, string comment)
    {
        // Создаём шаблон регулярного выражения.
        // Regex.Escape() обезопасит от спецсимволов в keyword.
        string pattern = $@"^\s*{Regex.Escape(keyword)}\s*:\s*(.*)";

        // Используем Regex.Match для поиска совпадения.
        // RegexOptions.IgnoreCase делает поиск нечувствительным к регистру.
        Match match = Regex.Match(comment, pattern, RegexOptions.IgnoreCase);

        // Проверяем, было ли совпадение
        if(match.Success)
        {
            // Возвращаем содержимое первой захваченной группы (.*) и удаляем пробелы
            return match.Groups[1].Value.Trim();
        }

        // Если совпадений нет, возвращаем null
        return null;
    }
}