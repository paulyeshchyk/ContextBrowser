using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlKit.Builders.Core;

// pattern: Decorator
// pattern note: weak
public static class HtmlBuilderExtensions
{
    public static async Task WithAsync(this IHtmlBuilder builder, TextWriter sb, Func<TextWriter, CancellationToken, Task>? body = default, CancellationToken cancellationToken = default)
    {
        await builder.StartAsync(sb, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (body != default)
        {
            await body(sb, cancellationToken).ConfigureAwait(false);
        }
        await builder.EndAsync(sb, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public static async Task WithAsync(this IHtmlTagBuilder builder, TextWriter sb, Func<CancellationToken, Task>? body = default, CancellationToken cancellationToken = default)
    {
        await builder.StartAsync(sb, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (body != default)
        {
            await body(cancellationToken).ConfigureAwait(false);
        }
        await builder.EndAsync(sb, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public static async Task WithAsync(this IHtmlBuilder builder, TextWriter writer, IHtmlTagAttributes? attributes, Func<CancellationToken, Task>? body = default, CancellationToken cancellationToken = default)
    {
        await builder.StartAsync(writer, attributes, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (body != default)
        {
            await body(cancellationToken).ConfigureAwait(false);
        }
        await builder.EndAsync(writer, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}