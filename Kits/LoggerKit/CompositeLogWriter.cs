using System;
using System.Collections.Generic;
using System.Linq;

namespace LoggerKit;

public class CompositeLogWriter : ILogWriter
{
    private readonly IEnumerable<ILogWriter> _writers;

    public CompositeLogWriter(params ILogWriter[] writers)
    {
        _writers = writers;
    }

    public void Write(string message)
    {
        foreach (var writer in _writers)
        {
            writer.Write(message);
        }
    }

    public void Dispose()
    {
        foreach (var writer in _writers.OfType<IDisposable>())
        {
            writer.Dispose();
        }
    }
}