namespace ContextBrowser.Extensions;

internal static class FileUtils
{
    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Директория '{path}' успешно создана.");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Ошибка ввода-вывода при создании директории '{path}': {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Отказано в доступе при создании директории '{path}': {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка при создании директории '{path}': {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Директория '{path}' уже существует.");
        }
    }

    public static void WipeDirectory(string path, bool shouldClearContentOnly = true)
    {
        if (shouldClearContentOnly)
        {
            ClearDirectoryContents(path);
        }
        else
        {
            DeleteDirectory(path);
        }
    }

    public static void ClearDirectoryContents(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Ошибка: Директория '{path}' не существует.");
            return;
        }

        try
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                DeleteFile(file);
            }

            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                DeleteDirectory(dir);
            }

            Console.WriteLine($"\nДиректория '{path}' успешно очищена.");
        }
        catch (Exception e)
        {
            // Общий обработчик для других возможных ошибок при получении списка файлов/директорий
            Console.WriteLine($"Произошла непредвиденная ошибка при очистке директории '{path}': {e.Message}");
        }
    }

    private static void DeleteFile(string file)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine($"Ошибка: Файл '{file}' не существует.");
            return;
        }
        try
        {
            File.Delete(file);
            Console.WriteLine($"Удален файл: {file}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка при удалении файла '{file}': {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Отказано в доступе при удалении файла '{file}': {ex.Message}");
        }
    }

    public static void DeleteDirectory(string path, bool recursive = true)
    {
        // Проверяем, существует ли директория
        if (Directory.Exists(path))
        {
            try
            {
                // Второй параметр 'true' указывает на рекурсивное удаление
                // всех файлов и поддиректорий внутри указанной директории.
                Directory.Delete(path, recursive);
                Console.WriteLine($"Директория '{path}' и её содержимое успешно удалены.");
            }
            catch (IOException e)
            {
                // Обработка ошибок ввода-вывода (например, если файл заблокирован)
                Console.WriteLine($"Ошибка ввода-вывода при удалении директории '{path}': {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                // Обработка ошибок доступа (например, если нет прав на удаление)
                Console.WriteLine($"Отказано в доступе при удалении директории '{path}': {e.Message}");
            }
            catch (Exception e)
            {
                // Обработка любых других непредвиденных ошибок
                Console.WriteLine($"Произошла непредвиденная ошибка при удалении директории '{path}': {e.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Директория '{path}' не существует.");
        }
    }
}