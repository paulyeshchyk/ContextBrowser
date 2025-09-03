using System;
using System.Collections.Generic;
using System.IO;

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

    public static void With(this IHtmlBuilder builder, TextWriter writer, IHtmlTagAttributes? attributes, Action body)
    {
        builder.Start(writer, attributes);
        body();
        builder.End(writer);
    }
}