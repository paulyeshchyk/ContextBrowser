using System;
using ContextBrowser;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Options;
using ContextBrowserKit;
using ContextBrowserKit.Commandline.Polyfills;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;

namespace ContextBrowser.Infrastructure.Options;

public record RunOptions
{
    [CommandLineArgument("project", "Название предустановленного проекта")] // Например: --project Gulf
    public string Project { get; set; } = "Default";

    [CommandLineArgument("appOptionsFilePath", "Путь и имя к конфигурационному файлу")]
    public string AppOptionsFilePath { get; set; } = "settings.json";

    [CommandLineArgument("renewAppOptions", "Записывает введённые значения в указанный конфигурационный файл")]
    public bool RenewAppOptions { get; set; } = false;

    [CommandLineArgument("appOptions", "Конфигурация")]
    public AppOptions? AppOptions { get; set; }

    public RunOptions()
    {
    }

    public RunOptions(string appOptionsFilePath, bool renewAppOptions)
    {
        AppOptionsFilePath = appOptionsFilePath;
        RenewAppOptions = renewAppOptions;
    }
}