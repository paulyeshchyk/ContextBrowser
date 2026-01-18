using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Options;
using ContextBrowser.Infrastructure.Options.JsonConverters;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit.Model;

namespace ContextBrowser.Infrastructure.Options.JsonConverters;

public class ContextClassifierConverter<TContext> : JsonConverter<IContextClassifier<TContext>>
    where TContext : IContextWithReferences<TContext>
{
    public override IContextClassifier<TContext> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Десериализуем в конкретный класс
        var concrete = JsonSerializer.Deserialize<ContextClassifier>(ref reader, options) as IContextClassifier<TContext> ?? throw new JsonException("Failed to deserialize ContextClassifier");
        return concrete;
    }

    public override void Write(Utf8JsonWriter writer, IContextClassifier<TContext> value, JsonSerializerOptions options)
    {
        // Записываем как конкретный объект
        JsonSerializer.Serialize(writer, (ContextClassifier)value, options);
    }
}
