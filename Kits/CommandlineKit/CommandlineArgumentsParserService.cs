using System;
using CommandlineKit.Model;

namespace CommandlineKit;

public interface ICommandlineArgumentsParserService
{
    T? Parse<T>(string[] args)
        where T : class;
}

// context: commandline, build
public class CommandlineArgumentsParserService : ICommandlineArgumentsParserService
{
    // context: commandline, build
    public T? Parse<T>(string[] args)
        where T : class
    {
        var parser = new CommandLineParser();

        if (!parser.TryParse<T>(args, out var options, out var errorMessage))
        {
            Console.WriteLine(errorMessage);
            return default;
        }

        if (options != null)
        {
            return options;
        }

        Console.WriteLine("Что-то пошло не так))");
        CommandLineHelpProducer.ShowHelp<T>(args);
        return default;
    }
}