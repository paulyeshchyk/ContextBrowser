using System.Collections.Generic;
using System.IO;
using UmlKit.Model;

namespace UmlKit.PlantUmlSpecification;

public class UmlStyleAttributes : IUmlElement
{
    private readonly Dictionary<string, string> _attributes = new();

    internal void Add(string key, string value) => _attributes.Add(key, value);

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        foreach (var attribute in _attributes.Keys)
        {
            writer.WriteLine($"{attribute} {this._attributes[attribute]}");
        }
    }
}
