using UmlKit.Extensions;

namespace UmlKit.Model;

public class UmlActivate : IUmlElement, IUmlDeclarable
{
    public readonly string? Source;
    public readonly string Destination;
    public readonly string _reason;
    public readonly bool SoftActivation;

    public UmlActivate(string? source, string destination, string reason, bool softActivation)
    {
        Destination = destination.AlphanumericOnly(replaceBy: "_");
        Source = source?.AlphanumericOnly(replaceBy: "_");
        _reason = reason;
        SoftActivation = softActivation;
    }

    public string Declaration => $"activate {this.Destination}";

    public string Alias => Destination;

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine();

        if(!string.IsNullOrEmpty(Source))
        {
            writer.Write(Source);
            writer.Write($" -> {Destination}");
            if(!string.IsNullOrWhiteSpace(_reason))
            {
                writer.Write($": {_reason}");
            }
            writer.WriteLine();
        }
        else
        {
            if(!string.IsNullOrWhiteSpace(_reason))
            {
                writer.Write($" -> {Destination}");
                writer.Write($": {_reason}");
                writer.WriteLine();
            }
        }

        if(!SoftActivation)
            writer.WriteLine(Declaration);
    }
}