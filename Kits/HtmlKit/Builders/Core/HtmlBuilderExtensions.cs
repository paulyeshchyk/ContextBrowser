namespace HtmlKit.Builders.Core;

// pattern: Decorator
// pattern note: weak
public static class HtmlBuilderExtensions
{
    public static void With(this IHtmlBuilder builder, TextWriter sb, Action<TextWriter> body)
    {
        builder.Start(sb);
        body(sb);
        builder.End(sb);
    }

    public static void With(this IHtmlTagBuilder builder, TextWriter sb, Action body)
    {
        builder.Start(sb);
        body();
        builder.End(sb);
    }

    public static void With(this IHtmlBuilder builder, TextWriter writer, Action body, string? className)
    {
        builder.Start(writer, className);
        body();
        builder.End(writer);
    }
}