﻿namespace ContextBrowser.Extensions;

public class PathAnalyzer
{
    public enum PathType
    {
        File,
        Directory,
        SymbolicLink, // Может быть файлом или папкой
        NonExistentPath,
        PlainText // Не является путем в файловой системе или не существует
    }

    public static PathType GetPathType(string path)
    {
        if(string.IsNullOrWhiteSpace(path))
        {
            return PathType.PlainText; // Или можно бросить ArgumentException, в зависимости от требований
        }

        try
        {
            // Проверяем, является ли это существующим файлом
            if(File.Exists(path))
            {
                // Дополнительная проверка на символическую ссылку (файл)
                FileAttributes attributes = File.GetAttributes(path);
                if((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    return PathType.SymbolicLink; // Символическая ссылка на файл
                }
                return PathType.File;
            }

            // Проверяем, является ли это существующей папкой
            if(Directory.Exists(path))
            {
                // Дополнительная проверка на символическую ссылку (папка)
                FileAttributes attributes = File.GetAttributes(path);
                if((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    return PathType.SymbolicLink; // Символическая ссылка на папку
                }
                return PathType.Directory;
            }

            // Если не существует как файл или папка, возможно, это просто текст или несуществующий путь
            // Попытка определить, похожа ли строка на путь
            if(path.Contains(Path.DirectorySeparatorChar) || path.Contains(Path.AltDirectorySeparatorChar) || Path.HasExtension(path))
            {
                return PathType.NonExistentPath;
            }
            else
            {
                return PathType.PlainText;
            }
        }
        catch(IOException)
        {
            // Например, "имя файла, имя каталога или синтаксис метки тома неверен"
            // В этом случае, это, скорее всего, недействительный путь или просто текст
            return PathType.PlainText;
        }
        catch(UnauthorizedAccessException)
        {
            // У пользователя нет разрешений для доступа к пути
            return PathType.NonExistentPath; // Или можно добавить PathType.NoPermission
        }
        catch(NotSupportedException)
        {
            // Путь в недопустимом формате (например, содержит двоеточие в середине имени файла)
            return PathType.PlainText;
        }
        catch(System.Security.SecurityException)
        {
            // Отсутствуют необходимые разрешения
            return PathType.NonExistentPath;
        }
    }
}