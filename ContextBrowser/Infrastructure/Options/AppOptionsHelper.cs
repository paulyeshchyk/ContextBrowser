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

namespace ContextBrowser.Infrastructure.Options;

public static class AppOptionsHelper
{
    public static async Task<AppOptions?> LoadFromFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<AppOptions>(json, JsonDefaults.Options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config from {path}: {ex.Message}");
            return null;
        }
    }

    public static async Task SaveToFile(string path, AppOptions options)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var json = JsonSerializer.Serialize(options, JsonDefaults.Options);
        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"Configuration saved to: {path}");
    }
}
