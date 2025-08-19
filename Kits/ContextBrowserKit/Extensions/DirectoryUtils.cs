using ContextBrowserKit.Log;

namespace ContextBrowserKit.Extensions;

// context: file, read, delete, create
public static class DirectoryUtils
{
    // context: file, read, delete, create
    public static void Prepare(string outputDirectory, OnWriteLog? onWriteLog = default)
    {
        FileUtils.CreateDirectoryIfNotExists(outputDirectory, onWriteLog: onWriteLog);
        FileUtils.WipeDirectory(outputDirectory, shouldClearContentOnly: true, onWriteLog: onWriteLog);
    }
}