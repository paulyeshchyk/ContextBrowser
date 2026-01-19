using System;
using LoggerKit;
using LoggerKit.Writers;

namespace LoggerKit.Writers;

// context: log, share
public class ConsoleLogWriter : ILogWriter
{
    // context: log, share
    public void Write(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;
        Console.WriteLine(message);
    }
}