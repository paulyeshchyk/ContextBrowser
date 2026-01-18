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

public class EmptyDimensionClassifierDomainPerActionConverter : JsonConverter<IEmptyDimensionClassifier>
{
    public override IEmptyDimensionClassifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Десериализуем в конкретный класс
        var concrete = JsonSerializer.Deserialize<EmptyDimensionClassifierDomainPerAction>(ref reader, options) as IEmptyDimensionClassifier ?? throw new JsonException("Failed to deserialize ContextClassifier");
        return concrete;
    }

    public override void Write(Utf8JsonWriter writer, IEmptyDimensionClassifier value, JsonSerializerOptions options)
    {
        // Записываем как конкретный объект
        JsonSerializer.Serialize(writer, (EmptyDimensionClassifierDomainPerAction)value, options);
    }
}
