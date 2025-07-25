
using ContextBrowser.UmlKit.Extensions;

namespace ContextBrowser.UmlKit.Model;

public class UmlState : IUmlElement, IUmlDeclarable
{
    private readonly string _raw;

    public string ShortName => _raw.StateShortName();

    public string FullName => _raw.StateFullName();

    public string Declaration => $"state {this.FullName}";

    public UmlState(string raw)
    {
        _raw = raw;
    }

    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine(Declaration);
    }
}
