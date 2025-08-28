using System.Text.RegularExpressions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;

namespace ContextBrowserKit.Extensions;

// context: directory, read
// coverage: 12
public class PathAnalyzer
{
    // context: directory, model
    public enum PathType
    {
        File,
        Directory,
        SymbolicLink, // Может быть файлом или папкой
        NonExistentPath,
        PlainText // Не является путем в файловой системе или не существует
    }

    // context: file, directory, read
    public static string[] GetFilePaths(string[] paths, string searchPattern, OnWriteLog? onWriteLog = null)
    {
        var filePaths = new HashSet<string>();

        // 1. Разделяем строку с паттернами на массив
        var patterns = searchPattern.Split(';');

        foreach (var path in paths)
        {
            try
            {
                // 2. Проверяем, является ли путь папкой
                if (Directory.Exists(path))
                {
                    // 3. Перебираем каждый паттерн
                    foreach (var pattern in patterns)
                    {
                        var validatedPattern = ValidatePattern(pattern);
                        if (string.IsNullOrWhiteSpace(validatedPattern))
                            continue;

                        var filesInDirectory = Directory.GetFiles(path, validatedPattern, SearchOption.AllDirectories);
                        foreach (var file in filesInDirectory)
                        {
                            filePaths.Add(file);
                        }
                    }
                }

                // 4. Добавляем путь, если это файл
                else if (File.Exists(path))
                {
                    if (patterns.Any(p => IsFileMatch(path, p)))
                    {
                        filePaths.Add(path);
                    }
                }
                else
                {
                    onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, $"Путь не найден: {path}");
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем возможные ошибки
                onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, $"Ошибка при обработке пути '{path}': {ex.Message}");
            }
        }

        return filePaths.ToArray();
    }

    /// <summary>
    /// Проверяет и корректирует шаблон поиска, добавляя '*' и '.' при необходимости.
    /// </summary>
    /// <param name="pattern">Исходный шаблон из командной строки.</param>
    /// <returns>Валидированный шаблон для Directory.GetFiles.</returns>
    private static string ValidatePattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return string.Empty;
        }

        // Удаляем пробелы, чтобы избежать ошибок
        pattern = pattern.Trim();

        // Если шаблон уже содержит '*', считаем, что он введён корректно
        if (pattern.Contains('*'))
        {
            return pattern;
        }

        // Добавляем точку и звёздочку для расширений файлов, например, "cs" -> "*.cs"
        if (!pattern.StartsWith("."))
        {
            pattern = "." + pattern;
        }

        // Если шаблон не начинается со звёздочки, добавляем её
        return "*" + pattern;
    }

    private static bool IsFileMatch(string filePath, string pattern)
    {
        var fileName = Path.GetFileName(filePath);
        var regexPattern = Regex.Escape(pattern) + "$"; // pattern == ".cs"
        return Regex.IsMatch(fileName, regexPattern, RegexOptions.IgnoreCase);
    }

    // context: directory, read
    public static PathType GetPathType(string path, OnWriteLog? onWriteLog = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return PathType.PlainText; // Или можно бросить ArgumentException, в зависимости от требований
        }

        try
        {
            // Проверяем, является ли это существующим файлом
            if (File.Exists(path))
            {
                // Дополнительная проверка на символическую ссылку1 (файл)
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    return PathType.SymbolicLink; // Символическая ссылка на файл
                }
                return PathType.File;
            }

            // Проверяем, является ли это существующей папкой
            if (Directory.Exists(path))
            {
                // Дополнительная проверка на символическую ссылку2 (папка)
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    return PathType.SymbolicLink; // Символическая ссылка на папку
                }
                return PathType.Directory;
            }

            // Если не существует как файл или папка, возможно, это просто текст или несуществующий путь
            // Попытка определить, похожа ли строка на путь
            if (path.Contains(Path.DirectorySeparatorChar) || path.Contains(Path.AltDirectorySeparatorChar) || Path.HasExtension(path))
            {
                return PathType.NonExistentPath;
            }
            else
            {
                return PathType.PlainText;
            }
        }
        catch (IOException)
        {
            // Например, "имя файла, имя каталога или синтаксис метки тома неверен"
            // В этом случае, это, скорее всего, недействительный путь или просто текст
            return PathType.PlainText;
        }
        catch (UnauthorizedAccessException)
        {
            // У пользователя нет разрешений для доступа к пути
            return PathType.NonExistentPath; // Или можно добавить PathType.NoPermission
        }
        catch (NotSupportedException)
        {
            // Путь в недопустимом формате (например, содержит двоеточие в середине имени файла)
            return PathType.PlainText;
        }
        catch (System.Security.SecurityException)
        {
            // Отсутствуют необходимые разрешения
            return PathType.NonExistentPath;
        }
    }
}
