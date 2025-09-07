using System.Collections.Generic;
using System.IO;
using System.Linq;
using UmlKit.PlantUmlSpecification.Attributes;

namespace UmlKit.Model;

public class UmlMethod : IUmlElement
{
    public readonly string Text;
    public readonly string? Url;
    public readonly UmlMemberVisibility Visibility;

    public UmlMethod(string text, UmlMemberVisibility Visibility, string? url)
    {
        Text = text;
        this.Visibility = Visibility;
        Url = url;
    }

    public void WriteTo(TextWriter writer, int alignNameMaxWidth)
    {
        List<string?> properties = new();
        properties.Add(Visibility.ToUmlString());
        var textToWrite = (alignNameMaxWidth > 0)
            ? Text.PadRight(alignNameMaxWidth)
            : Text;
        properties.Add($"{textToWrite}");

        //properties.Add(MethodAttributesBuilder.BuildUrl(Url));

        var result = string.Join(" ", properties.Cast<string>());
        writer.WriteLine(result);
    }
}

internal static class UmlMethodVisibilityExt
{
    public static string ToUmlString(this UmlMemberVisibility visiblity)
    {
        return visiblity switch
        {
            UmlMemberVisibility.@public => "+",
            UmlMemberVisibility.@private => "-",
            UmlMemberVisibility.@protected => "#",
            UmlMemberVisibility.@static => "{static}",
            _ => string.Empty
        };
    }
}

public enum UmlMemberVisibility
{
    @public,
    @private,
    @protected,
    @static
}