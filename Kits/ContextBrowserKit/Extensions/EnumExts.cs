namespace UmlKit.Extensions;

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