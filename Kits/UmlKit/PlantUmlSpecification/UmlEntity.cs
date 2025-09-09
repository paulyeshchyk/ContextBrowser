using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.PlantUmlSpecification.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace UmlKit.Model;

// context: model, uml
// pattern: Composite leaf
public class UmlEntity : IUmlElement, IUmlDeclarable, IUmlElementCollection
{
    private const string SFakeClassName = "FakeClassName";

    protected string _type => UmlEntityTypeExt.ConvertToString(EntityType);

    public string Name { get; }

    public string Alias { get; }

    public SortedList<int, IUmlElement> Elements { get; } = new();

    public UmlEntityType EntityType { get; }

    public string? Url { get; }

    public string Declaration => $"{_type} \"{Name}\" as {Alias}";

    public UmlEntity(UmlEntityType entityType, string? name, string alias, string? url = null)
    {
        EntityType = entityType;
        Name = name ?? SFakeClassName;
        Alias = alias;
        Url = url;
    }

    public void Add(IUmlElement e) => Elements.Add(Elements.Count, e);

    // context: uml, share
    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        writer.WriteLine();
        writer.WriteLine($"{Declaration} {ClassAttributesBuilder.BuildUrl(Url)}");
        writer.WriteLine("{");
        foreach (var element in Elements.OrderBy(e => e.Key).Select(e => e.Value))
            element.WriteTo(writer, writeOptions);
        writer.WriteLine("}");
    }
}

public enum UmlEntityType
{
    @none,
    @class,
    @record,
    @struct,
    @enum,
    @interface,
    @delegate,
    @method,
    @namespace,
    @property
}

public static class UmlEntityTypeExt
{
    public static string ConvertToString(this UmlEntityType entityType)
    {
        return entityType switch
        {
            UmlEntityType.@class => "class",
            UmlEntityType.@record => "record",
            UmlEntityType.@struct => "struct",
            UmlEntityType.@enum => "enum",
            UmlEntityType.@interface => "interface",
            UmlEntityType.@delegate => "metaclass",
            UmlEntityType.@method => "metaclass",

            UmlEntityType.@none => "metaclass",
            UmlEntityType.@namespace => "metaclass",
            UmlEntityType.@property => "metaclass",
            _ => throw new NotImplementedException()
        };
    }
}