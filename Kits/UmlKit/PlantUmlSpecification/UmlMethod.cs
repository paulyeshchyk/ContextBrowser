using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UmlKit.PlantUmlSpecification;

public class UmlMethod : IUmlElement
{
    public readonly string Text;
    public readonly string? Url;
    public readonly UmlMemberVisibility Visibility;

    public UmlMethod(string text, UmlMemberVisibility visibility, string? url)
    {
        Text = text;
        Visibility = visibility;
        Url = url;
    }

    public void WriteTo(TextWriter writer, UmlWriteOptions writeOptions)
    {
        List<string?> properties = new();
        properties.Add(Visibility.ToUmlString());
        var textToWrite = (writeOptions.AlignMaxWidth > 0)
            ? Text.PadRight(writeOptions.AlignMaxWidth)
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