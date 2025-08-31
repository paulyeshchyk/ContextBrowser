using CommandlineKit.Model;

namespace CommandlineKit;

public interface ICommandlineArgumentsParserService
{
    T? Parse<T>(string[] args);
}

public class CommandlineArgumentsParserService : ICommandlineArgumentsParserService
{
    public T? Parse<T>(string[] args)
    {
        var parser = new CommandLineParser();

        if (!parser.TryParse<T>(args, out var options, out var errorMessage))
        {
            Console.WriteLine(errorMessage);
            return default;
        }
        if (options == null)
        {
            Console.WriteLine("Что-то пошло не так))");
            Console.WriteLine(CommandLineHelpProducer.GenerateHelpText<T>(CommandLineDefaults.SArgumentPrefix));
            return default;
        }
        return options;
    }
}