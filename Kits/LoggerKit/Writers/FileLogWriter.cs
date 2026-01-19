using System;
using System.IO;
using LoggerKit;
using LoggerKit.Writers;

namespace LoggerKit.Writers;

public class FileLogWriter : ILogWriter, IDisposable
{
    private readonly string? _filePath;
    private readonly object _lock = new();
    private StreamWriter? _streamWriter;
    private bool _isAccessDenied = false;

    public FileLogWriter(string outputPath)
    {
        try
        {
            var dirName = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            var postfix = DateTime.Now.ToString("yyyyMMddHHmmss");
            _filePath = Path.Combine(outputPath, $"{postfix}.log");
        }
        catch (Exception ex)
        {
            _isAccessDenied = true;

            Console.WriteLine($"[CRITICAL] FileLogWriter access error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _streamWriter?.Dispose();
    }

    public void Write(string message)
    {
        if (_isAccessDenied || _filePath == null) 
            return;

        lock (_lock)
        {
            try
            {
                _streamWriter ??= new StreamWriter(_filePath, append: true) { AutoFlush = true };
                _streamWriter.WriteLine(message);
            }
            catch (Exception ex)
            {
                _isAccessDenied = true;
                Console.WriteLine($"[CRITICAL] Failed writing to log file: {ex.Message}");
            }
        }
    }
}