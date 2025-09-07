using System;
using System.Collections.Generic;
using System.IO;

namespace HtmlKit.Builders.Core;

// pattern: Decorator
// pattern note: weak
public static class HtmlBuilderExtensions
{
    public static void With(this IHtmlBuilder builder, TextWriter sb, Action<TextWriter>? body = default)
    {
        builder.Start(sb);
        if (body != default)
        {
            body(sb);
        }
        builder.End(sb);
    }

    public static void With(this IHtmlTagBuilder builder, TextWriter sb, Action? body = default)
    {
        builder.Start(sb);
        if (body != default)
        {
            body();
        }
        builder.End(sb);
    }

    public static void With(this IHtmlBuilder builder, TextWriter writer, IHtmlTagAttributes? attributes, Action? body = default)
    {
        builder.Start(writer, attributes);
        if (body != default)
        {
            body();
        }
        builder.End(writer);
    }
}