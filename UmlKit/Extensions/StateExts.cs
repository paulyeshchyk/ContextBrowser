namespace ContextBrowser.UmlKit.Extensions;

internal static class StateExts
{
    public static string StateFullName(this string s)
    {
        return $"\"{s}\" as {s.StateShortName()}";
    }

    public static string StateShortName(this string s)
    {
        return s.Replace(".", "_").Replace(" ", "_");
    }
}

internal static class EnumExts
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