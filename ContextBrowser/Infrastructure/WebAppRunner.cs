using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace ContextBrowser.Infrastructure;

internal static class WebAppRunner
{
    public static async Task Run(string[] args, AppOptions options)
    {
        var customUrl = options.Export.WebPaths.OutputDirectory;
        (string host, int port) = ParseUrl(customUrl);

        var builder = WebApplication.CreateBuilder(args);


        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            // 1. Сброс предыдущих настроек конечных точек
            serverOptions.ListenAnyIP(0); // Сбрасывает любые адреса, которые Kestrel мог загрузить

            // 2. Явная установка конечной точки
            serverOptions.Listen(IPAddress.Parse(host == "localhost" ? "127.0.0.1" : host), port);

            // Если вы хотите всегда использовать localhost:
            // serverOptions.ListenLocalhost(port);

            // Если вы хотите HTTPS, здесь нужно добавить конфигурацию сертификата
            // serverOptions.Listen(IPAddress.Parse(host), port, listenOptions => { listenOptions.UseHttps(); });
        });

        HostConfigurator.ConfigureServices(builder.Services);

        // Добавляем специфичные для Web API службы
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // 1. Получаем IHostApplicationLifetime для выполнения кода после старта
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        // 2. Регистрируем действие, которое будет выполнено, когда хост полностью запустится
        lifetime.ApplicationStarted.Register(() =>
        {
            try
            {
                // Формируем полный URL (например, "http://localhost:5500/ContextBrowserStartPage.html")
                var startUrl = customUrl.TrimEnd('/') + "/ContextBrowserStartPage.html";

                // Используем Process.Start для открытия URL в браузере по умолчанию
                // Это кроссплатформенный способ, работающий на Windows, Linux и macOS.
                Process.Start(new ProcessStartInfo
                {
                    FileName = startUrl,
                    UseShellExecute = true
                });

                // // *Опционально: Вывод для подтверждения*
                // var logger = app.Services.GetRequiredService<IAppLogger<AppLevel>>();
                // logger.Log(AppLevel.App, LogLevel.Info, $"Automatically opening browser at: {startUrl}");
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, если браузер не найден или нет прав)
                Console.WriteLine($"Error launching browser: {ex.Message}");
            }
        });


        // --- Настройка HTTP-конвейера ---
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles(); // Разрешаем доступ к сгенерированным HTML
        app.UseAuthorization();
        app.MapControllers(); // Включаем контроллеры (API)

        // Устанавливаем опции приложения (если нужно)
        var optionsStore = app.Services.GetRequiredService<IAppOptionsStore>();
        optionsStore.SetOptions(options);

        await app.RunAsync();
    }

    private static (string Host, int Port) ParseUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return (uri.Host, uri.Port);
        }
        // Fallback, если не удалось распарсить
        return ("localhost", 5000);
    }
}
