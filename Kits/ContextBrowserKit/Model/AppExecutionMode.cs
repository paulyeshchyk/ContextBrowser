using System;
using System.ComponentModel;

namespace ContextBrowserKit.Model;

public enum AppExecutionMode
{
    [Description("Запуск в режиме веб-интерфейса (порт 5500)")]
    WebApp = 0,

    [Description("Классический консольный запуск анализатора")]
    Console = 1
}