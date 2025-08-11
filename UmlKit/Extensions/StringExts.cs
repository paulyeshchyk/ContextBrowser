using System.Text.RegularExpressions;

namespace UmlKit.Extensions;

public static class StringExts
{
    public static string AlphanumericOnly(this string s, string replaceBy = "_") => Regex.Replace(s, "[^a-zA-Z0-9]", replaceBy);
}

public static class EnumExts
{
    public static string ConvertToString(this Enum eff)
    {
        return Enum.GetName(eff.GetType(), eff) ?? throw new ArgumentException($"некорректное перечисление: {nameof(eff)}");
    }

    public static EnumType ConverToEnum<EnumType>(this string enumValue)
    {
        return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
    }
}