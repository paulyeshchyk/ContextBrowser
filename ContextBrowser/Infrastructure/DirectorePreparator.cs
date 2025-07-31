using ContextBrowser.Extensions;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.Infrastructure;

// context: file, read, delete, create
public static class DirectorePreparator
{
    // context: file, read, delete, create
    public static void Prepare(AppOptions options, OnWriteLog? onWriteLog = default)
    {
        FileUtils.CreateDirectoryIfNotExists(options.outputDirectory, onWriteLog: onWriteLog);
        FileUtils.WipeDirectory(options.outputDirectory, onWriteLog: onWriteLog);
    }
}
