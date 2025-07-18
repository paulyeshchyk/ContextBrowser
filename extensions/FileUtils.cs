namespace ContextBrowser.Extensions;

internal static class FileUtils
{
    private const string SFolderCreatedTemplate = "Папка '{0}' успешно создана.";
    private const string SErrorThrowenForDirectoryCreationTemplate = "Ошибка ввода-вывода при создании папки '{0}': {1}";
    private const string SNFolderAccessTemplate = "Отказано в доступе при создании папки '{0}': {1}";
    private const string SUnknownErrorTemplate = "Произошла непредвиденная ошибка при создании папки '{0}': {1}";
    private const string SFolderExistsTemplate = "Папка '{0}' уже существует.";
    private const string SFolderNotFoundTemplate = "Ошибка: Папка '{0}' не существует.";
    private const string SFolderWipedTemplate = "\nПапка '{0}' успешно очищена.";
    private const string SUnknownErrorWipeTemplate = "Произошла непредвиденная ошибка при очистке папки '{0}': {1}";
    private const string SFileNotFoundTemplate = "Ошибка: Файл '{0}' не существует.";
    private const string SFileDeletedTemplate = "Удален файл: {0}";
    private const string SFileDeleteErrorTemplate = "Ошибка при удалении файла '{0}': {1}";
    private const string SFileDeleteNoAccessTemplate = "Отказано в доступе при удалении файла '{0}': {1}";
    private const string SFolderWipedSuccessfullyTemplate = "Папка '{0}' и её содержимое успешно удалены.";
    private const string SFolderWipeErrorTemplate = "Ошибка ввода-вывода при удалении папки '{0}': {1}";
    private const string SFolderWipeAccessErrorTemplate = "Отказано в доступе при удалении папки '{0}': {1}";
    private const string SFolderWipeUnknownErrorTemplate = "Произошла непредвиденная ошибка при удалении папки '{0}': {1}";

    public static void CreateDirectoryIfNotExists(string path)
    {
        if(Directory.Exists(path))
        {
            Console.WriteLine(string.Format(SFolderExistsTemplate, path));
            return;
        }

        try
        {
            Directory.CreateDirectory(path);
            Console.WriteLine(string.Format(SFolderCreatedTemplate, path));
        }
        catch(IOException e)
        {
            Console.WriteLine(string.Format(SErrorThrowenForDirectoryCreationTemplate, path, e.Message));
        }
        catch(UnauthorizedAccessException e)
        {
            Console.WriteLine(string.Format(SNFolderAccessTemplate, path, e.Message));
        }
        catch(Exception e)
        {
            Console.WriteLine(string.Format(SUnknownErrorTemplate, path, e.Message));
        }
    }

    public static void WipeDirectory(string path, bool shouldClearContentOnly = true)
    {
        if(shouldClearContentOnly)
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
        if(!Directory.Exists(path))
        {
            Console.WriteLine(string.Format(SFolderNotFoundTemplate, path));
            return;
        }

        try
        {
            string[] files = Directory.GetFiles(path);
            foreach(string file in files)
            {
                DeleteFile(file);
            }

            string[] directories = Directory.GetDirectories(path);
            foreach(string dir in directories)
            {
                DeleteDirectory(dir);
            }

            Console.WriteLine(string.Format(SFolderWipedTemplate, path));
        }
        catch(Exception e)
        {
            Console.WriteLine(string.Format(SUnknownErrorWipeTemplate, path, e.Message));
        }
    }

    private static void DeleteFile(string file)
    {
        if(!File.Exists(file))
        {
            Console.WriteLine(string.Format(SFileNotFoundTemplate, file));
            return;
        }

        try
        {
            File.Delete(file);
            Console.WriteLine(string.Format(SFileDeletedTemplate, file));
        }
        catch(IOException ex)
        {
            Console.WriteLine(string.Format(SFileDeleteErrorTemplate, file, ex.Message));
        }
        catch(UnauthorizedAccessException ex)
        {
            Console.WriteLine(string.Format(SFileDeleteNoAccessTemplate, file, ex.Message));
        }
    }

    public static void DeleteDirectory(string path, bool recursive = true)
    {
        if(!Directory.Exists(path))
        {
            Console.WriteLine(string.Format(SFolderExistsTemplate, path));
            return;
        }

        try
        {
            Directory.Delete(path, recursive);
            Console.WriteLine(string.Format(SFolderWipedSuccessfullyTemplate, path));
        }
        catch(IOException e)
        {
            Console.WriteLine(string.Format(SFolderWipeErrorTemplate, path, e.Message));
        }
        catch(UnauthorizedAccessException e)
        {
            Console.WriteLine(string.Format(SFolderWipeAccessErrorTemplate, path, e.Message));
        }
        catch(Exception e)
        {
            Console.WriteLine(string.Format(SFolderWipeUnknownErrorTemplate, path, e.Message));
        }
    }
}