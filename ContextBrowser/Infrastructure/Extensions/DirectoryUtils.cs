using LoggerKit;
using RoslynKit.Extensions;

namespace ContextBrowser.Infrastructure.Extensions;

// context: file, read, delete, create
public static class DirectoryUtils
{
    // context: file, read, delete, create
    public static void Prepare(AppOptions options, OnWriteLog? onWriteLog = default)
    {
        FileUtils.CreateDirectoryIfNotExists(options.outputDirectory, onWriteLog: onWriteLog);
        FileUtils.WipeDirectory(options.outputDirectory, onWriteLog: onWriteLog);
    }
}