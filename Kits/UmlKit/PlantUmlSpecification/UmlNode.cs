using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Extensions;

namespace UmlKit.PlantUmlSpecification;

public class UmlNode : IUmlParticipant
{
    private const string SUnknownState = "unknown_state";
    protected readonly string _raw;

    public string Alias => _raw.AlphanumericOnly();

    public string FullName => $"{_raw}";

    public string Declaration => $"{this.FullName}";

    public string? Url { get; }

    public string Stylename { get; set; } = "green";

    public List<UmlNode> Children { get; } = new List<UmlNode>();

    public List<UmlNode> Parents { get; } = new List<UmlNode>();

    public string Id { get; } = Guid.NewGuid().ToString();

    public UmlNode(string? raw, string? alias, string? url = null)
    {
        _raw = string.IsNullOrWhiteSpace(raw) ? SUnknownState : raw;
        Url = url;
    }

    private record data
    {
        public string Name { get; set; } = string.Empty;

        public string? Url { get; set; } = string.Empty;

        public int Depth { get; set; } = 0;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        var data = new data() { Name = FullName, Url = Url, Depth = 1 };
        var draw = DrawNode(writeOptions, data);
        writer.WriteLine(draw);

        foreach (var child in Children)
        {
            child.WriteTo(writer, new UmlWriteOptions(writeOptions.AlignMaxWidth, 1 + writeOptions.DepthIndex));
        }

        //if (Parents.Any())
        //{
        //    writer.WriteLine("left side");
        //}

        //foreach (var parent in Parents)
        //{
        //    parent.WriteTo(writer, new UmlWriteOptions(writeOptions.AlignMaxWidth, 1 + writeOptions.DepthIndex));
        //}
    }

    private string DrawNode(UmlWriteOptions writeOptions, data data)
    {
        var list = new List<string?>();

        //будем трактовать writeOptions.DepthIndex == 0 как единицу
        var jointSymbols = Enumerable.Repeat<string>("*", writeOptions.DepthIndex + data.Depth);

        //получаем строчку ****, 4 символа == writeOptions.DepthIndex + 1
        var nodeJoint = string.Join(string.Empty, jointSymbols);

        list.Add(nodeJoint);

        if (Url != null)
        {
            // [[http://www.google.com Поисковая_система]]
            list.Add($"[[{data.Url} {data.Name.AlphanumericOnly()}]]");
        }
        else
        {
            // Поисковая_система
            list.Add($"{data.Name.AlphanumericOnly()}");
        }

        // style: <<brown>>
        list.Add($"<< {this.Stylename} >>");

        var result = string.Join(" ", list.Cast<string>().Where(s => !string.IsNullOrWhiteSpace(s)));

        return result;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_raw);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        UmlNode other = (UmlNode)obj;
        return _raw.Equals(other._raw);
    }
}