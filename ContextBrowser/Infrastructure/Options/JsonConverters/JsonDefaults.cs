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

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        Converters = {
            new JsonStringEnumConverter(),
            new ContextClassifierConverter<ContextInfo>(),
            new EmptyDimensionClassifierDomainPerActionConverter(),
            new FakeDimensionClassifierDomainPerActionConverter(),
            new DomainPerActionContextTensorClassifierConverter<ContextInfo>(),
        },

        // .NET 8+ : ReadOnly коллекции
        //PreferredPropertyObjectCreationHandling = JsonObjectCreationHandling.Populate 
    };
}
