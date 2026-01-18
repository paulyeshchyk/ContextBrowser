using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser.Infrastructure.Options.Projects;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;

namespace ContextBrowser.Infrastructure.Options;

public static class AppOptionsResolver
{
    private static object? GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

    public static async Task<AppOptions?> ResolveOptionsAsync(string[] args, CancellationToken cancellationToken)
    {
        // Создаем временный объект для анализа типов и дефолтных значений
        var defaultRunOptions = new RunOptions
        {
            AppOptions = new AppOptions()
        };

        if (CommandLineHelpProducer.ShowHelp(args, defaultRunOptions))
        {
            return null;
        }

        var logger = AppLoggerFactory.DefaultIndentedLogger<AppLevel>();
        logger.Configure(AppOptions.DefaultLogConfiguration());

        var parser = new CommandlineArgumentsParserService();
        var runOptions = parser.Parse<RunOptions>(args) ?? new RunOptions();

        // 1. Определение фундамента (Project VS File VS Defaults)
        AppOptions finalOptions = await ResolveBaseOptions(runOptions);
        logger.WriteLog(AppLevel.App, LogLevel.Dbg, "Parsing command line");

        // 2. Глубокий CLI Overlay
        // Парсим AppOptions из консоли. Допустим, там только --DiagramBuilder.Debug true
        var cliOptions = parser.Parse<AppOptions>(args);
        if (cliOptions != null)
        {
            DeepMergeOptions(finalOptions, cliOptions, logger);
            logger.WriteLog(AppLevel.App, LogLevel.Dbg, "[Config] Deep CLI overrides applied.");
        }

        // 3. Сохранение (Renew)
        if (runOptions.RenewAppOptions && !string.IsNullOrEmpty(runOptions.AppOptionsFilePath))
        {
            // Создаем директорию, если её нет
            var dir = Path.GetDirectoryName(runOptions.AppOptionsFilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            await AppOptionsHelper.SaveToFile(runOptions.AppOptionsFilePath, finalOptions);
            logger.WriteLog(AppLevel.App, LogLevel.Dbg, $"[Config] Compiled configuration saved to: {runOptions.AppOptionsFilePath}");
        }

        return finalOptions;
    }

    private static async Task<AppOptions> ResolveBaseOptions(RunOptions run)
    {
        // Приоритет 1: Явный проект
        if (!string.IsNullOrEmpty(run.Project) && run.Project != "Default")
        {
            return AppOptionsFactory.CreateDefault(run.Project);
        }

        // Приоритет 2: Файл
        if (!string.IsNullOrEmpty(run.AppOptionsFilePath) && File.Exists(run.AppOptionsFilePath))
        {
            return await AppOptionsHelper.LoadFromFile(run.AppOptionsFilePath) ?? new AppOptions();
        }

        // Приоритет 3: Внутренние дефолты
        return new AppOptions();
    }

    private static void MergeOptions(AppOptions target, AppOptions overlay)
    {
        if (overlay == null)
            return;

        foreach (var prop in typeof(AppOptions).GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var overlayValue = prop.GetValue(overlay);

            // Логика: переносим только если в overlay значение отличается от дефолта
            // Для ссылочных типов (классы настроек) это проверка на null
            if (overlayValue != null)
            {
                // Для значимых типов (enum, bool) сложнее, но если вы не вводили их в CLI, 
                // парсер часто оставляет их дефолтными.
                prop.SetValue(target, overlayValue);
            }
        }
    }

    private static void DeepMergeOptions(object target, object overlay, IAppLogger<AppLevel> logger, string path = "")
    {
        if (target == null || overlay == null)
            return;

        var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var targetValue = prop.GetValue(target);
            var overlayValue = prop.GetValue(overlay);

            if (overlayValue == null)
                continue;

            string currentPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
            bool isComplex = prop.PropertyType.IsClass && prop.PropertyType != typeof(string);

            if (isComplex)
            {
                if (targetValue == null)
                {
                    prop.SetValue(target, overlayValue);
                    logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[Config] Set complex property: {currentPath}");
                }
                else
                {
                    DeepMergeOptions(targetValue, overlayValue, logger, currentPath);
                }
            }
            else
            {
                // Проверка на изменение значения
                if (!Equals(overlayValue, GetDefault(prop.PropertyType)) && !Equals(overlayValue, targetValue))
                {
                    prop.SetValue(target, overlayValue);
                    logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"[Config] Override: {currentPath} | '{targetValue}' -> '{overlayValue}'");
                }
            }
        }
    }
}