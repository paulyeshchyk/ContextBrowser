using System;
using CommandlineKit.Model;

namespace CommandlineKit;

public interface ICommandlineArgumentsParserService
{
    T? Parse<T>(string[] args);
}

// context: commandline, build
public class CommandlineArgumentsParserService : ICommandlineArgumentsParserService
{
    // context: commandline, build
    public T? Parse<T>(string[] args)
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
        Console.WriteLine(CommandLineHelpProducer.GenerateHelpText<T>(CommandLineDefaults.SArgumentPrefix));
        return default;
    }
}