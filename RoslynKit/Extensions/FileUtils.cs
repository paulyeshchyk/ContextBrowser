using LoggerKit;
using LoggerKit.Model;

namespace RoslynKit.Extensions;

// context: read, file
public static class FileUtils
{
    private const string SFolderCreatedTemplate = "Папка '{0}' успешно создана.";
    private const string SErrorThrowenForDirectoryCreationTemplate = "Ошибка ввода-вывода при создании папки '{0}': {1}";
    private const string SNFolderAccessTemplate = "Отказано в доступе при создании папки '{0}': {1}";
    private const string SUnknownErrorTemplate = "Произошла непредвиденная ошибка при создании папки '{0}': {1}";
    private const string SFolderExistsTemplate = "Папка '{0}' уже существует.";
    private const string SFolderNotFoundTemplate = "Ошибка: Папка '{0}' не существует.";
    private const string SFolderWipedTemplate = "Папка '{0}' успешно очищена.";
    private const string SUnknownErrorWipeTemplate = "Произошла непредвиденная ошибка при очистке папки '{0}': {1}";
    private const string SFileErasedTemplate = "Файл '{0}' удалён.";
    private const string SFileNotFoundTemplate = "Ошибка: Файл '{0}' не существует.";
    private const string SFileDeleteErrorTemplate = "Ошибка при удалении файла '{0}': {1}";
    private const string SFileDeleteNoAccessTemplate = "Отказано в доступе при удалении файла '{0}': {1}";
    private const string SFolderWipedSuccessfullyTemplate = "Папка '{0}' и её содержимое успешно удалены.";
    private const string SFolderWipeErrorTemplate = "Ошибка ввода-вывода при удалении папки '{0}': {1}";
    private const string SFolderWipeAccessErrorTemplate = "Отказано в доступе при удалении папки '{0}': {1}";
    private const string SFolderWipeUnknownErrorTemplate = "Произошла непредвиденная ошибка при удалении папки '{0}': {1}";

    //context: file, create
    public static void CreateDirectoryIfNotExists(string path, OnWriteLog? onWriteLog = null)
    {
        if(Directory.Exists(path))
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Warn, string.Format(SFolderExistsTemplate, path));
            return;
        }

        try
        {
            Directory.CreateDirectory(path);
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Info, string.Format(SFolderCreatedTemplate, path));
        }
        catch(IOException e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SErrorThrowenForDirectoryCreationTemplate, path, e.Message));
        }
        catch(UnauthorizedAccessException e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SNFolderAccessTemplate, path, e.Message));
        }
        catch(Exception e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SUnknownErrorTemplate, path, e.Message));
        }
    }

    //context: file, delete, single
    public static void WipeDirectory(string path, bool shouldClearContentOnly = true, OnWriteLog? onWriteLog = default)
    {
        if(shouldClearContentOnly)
        {
            ClearDirectoryContents(path, onWriteLog: onWriteLog);
        }
        else
        {
            DeleteDirectory(path, onWriteLog: onWriteLog);
        }
    }

    //context: delete, file
    public static void ClearDirectoryContents(string path, OnWriteLog? onWriteLog = default)
    {
        if(!Directory.Exists(path))
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Warn, string.Format(SFolderNotFoundTemplate, path));
            return;
        }

        try
        {
            string[] files = Directory.GetFiles(path);
            foreach(string file in files)
            {
                DeleteFile(file, onWriteLog: onWriteLog);
            }

            string[] directories = Directory.GetDirectories(path);
            foreach(string dir in directories)
            {
                DeleteDirectory(dir, onWriteLog: onWriteLog);
            }

            onWriteLog?.Invoke(AppLevel.file, LogLevel.Info, string.Format(SFolderWipedTemplate, path));
        }
        catch(Exception e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SUnknownErrorWipeTemplate, path, e.Message));
        }
    }

    //context: delete, file
    private static void DeleteFile(string file, OnWriteLog? onWriteLog = default)
    {
        if(!File.Exists(file))
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Warn, string.Format(SFileNotFoundTemplate, file));
            return;
        }

        try
        {
            File.Delete(file);
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Warn, string.Format(SFileErasedTemplate, file));
        }
        catch(IOException ex)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SFileDeleteErrorTemplate, file, ex.Message));
        }
        catch(UnauthorizedAccessException ex)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SFileDeleteNoAccessTemplate, file, ex.Message));
        }
    }

    //context: file, delete, single
    public static void DeleteDirectory(string path, bool recursive = true, OnWriteLog? onWriteLog = default)
    {
        if(!Directory.Exists(path))
        {
            Console.WriteLine(string.Format(SFolderExistsTemplate, path));
            return;
        }

        try
        {
            Directory.Delete(path, recursive);
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Info, string.Format(SFolderWipedSuccessfullyTemplate, path));
        }
        catch(IOException e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SFolderWipeErrorTemplate, path, e.Message));
        }
        catch(UnauthorizedAccessException e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SFolderWipeAccessErrorTemplate, path, e.Message));
        }
        catch(Exception e)
        {
            onWriteLog?.Invoke(AppLevel.file, LogLevel.Err, string.Format(SFolderWipeUnknownErrorTemplate, path, e.Message));
        }
    }
}