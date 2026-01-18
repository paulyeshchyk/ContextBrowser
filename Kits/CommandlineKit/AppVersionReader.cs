using System;
using System.Reflection;

namespace CommandlineKit;

public static class AppVersionReader
{
    public static string GetVersionFromGit()
    {
        // Получаем информационную версию (она включает патчи, суффиксы и т.д.)
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return version ?? "0.0.0.1";
    }
}