using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ContextBrowser.ContextCommentsParser;

public abstract class CustomServer
{
    public abstract Process? StartServer(string filename, string arguments);

    public abstract void StopProcess(Process? process);

    public abstract void CopyFile(string sourceFile, string outputDirectory);

    public abstract void MkDir(string outputDirectory);

    public abstract void CopyResource(string resourceName, string folderPath);

    public abstract void CopyResourceX(byte[] resource, string name, string folderPath);

    public abstract bool IsPortInUse(int port);

    public abstract bool IsJvmPlantUmlProcessRunning(string jarFilename);
}

public static class CustomServerDetector
{
    public static OSPlatform GetOSPlatform()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }
        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        if(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) // || RuntimeInformation.IsOSPlatform(OSPlatform.OpenBSD) || RuntimeInformation.IsOSPlatform(OSPlatform.NetBSD)
        {
            return OSPlatform.FreeBSD;
        }

        // Если операционная система не определена
        throw new NotSupportedException("Операционная система не поддерживается.");
    }
}

public class WindowsServer : CustomServer
{
    public override void CopyResourceX(byte[] resource, string name, string folderPath)
    {
        Directory.CreateDirectory(folderPath);

        string destinationPath = Path.Combine(folderPath, name);
        File.WriteAllBytes(destinationPath, resource);
    }

    public override void CopyResource(string resourceName, string folderPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));

        if(fullResourceName == null)
        {
            Console.WriteLine($"Ресурс '{resourceName}' не найден.");
            return;
        }

        Directory.CreateDirectory(folderPath);

        string destinationPath = Path.Combine(folderPath, resourceName);
        using(Stream? resourceStream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if(resourceStream == null)
            {
                Console.WriteLine($"Не удалось получить поток для ресурса '{fullResourceName}'.");
                return;
            }

            using(FileStream fileStream = File.Create(destinationPath))
            {
                resourceStream.CopyTo(fileStream);
            }
        }

        Console.WriteLine($"Ресурс '{resourceName}' успешно скопирован в '{destinationPath}'.");
    }

    public override void CopyFile(string sourceFile, string outputDirectory)
    {
        string arguments = $"/c copy \"{sourceFile}\" \"{outputDirectory}\"";

        StartServer("cmd.exe", arguments);
    }

    public override void MkDir(string outputDirectory)
    {
        string arguments = $"/c md \"{outputDirectory}\"";

        StartServer("cmd.exe", arguments);
    }

    public override Process? StartServer(string filename, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = filename,
            Arguments = arguments,
            UseShellExecute = true
        };

        try
        {
            var process = Process.Start(startInfo);
            return process;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске процесса {filename}: {ex.Message}");
            return null;
        }
    }

    public override void StopProcess(Process? process)
    {
        if(process != null && !process.HasExited)
        {
            Console.WriteLine($"Остановка процесса с ID {process.Id}...");
            try
            {
                process.Kill();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Не удалось завершить процесс: {ex.Message}");
            }
        }
    }


    // Метод для проверки, занят ли порт
    public override bool IsPortInUse(int port)
    {
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] tcpConnections = ipGlobalProperties.GetActiveTcpListeners();

        return tcpConnections.Any(endpoint => endpoint.Port == port);
    }

    public override bool IsJvmPlantUmlProcessRunning(string jarFilename)
    {
        Process[] javaProcesses = Process.GetProcessesByName("java");

        foreach(var process in javaProcesses)
        {
            try
            {
                if(process.HasExited)
                    continue;

                string? executablePath = process.MainModule?.FileName;

                if(executablePath != null && executablePath.Contains(jarFilename))
                {
                    return true;
                }
            }
            catch(Exception)
            {
                // Игнорируем ошибки доступа, если нет прав
            }
        }
        return false;
    }
}

public class MacOsServer : CustomServer
{
    public override Process? StartServer(string filename, string arguments)
    {
        // На macOS, чтобы запустить команду в отдельном окне терминала,
        // мы используем команду `open -a Terminal`.
        var shellArguments = $"-a Terminal {filename} --args {arguments}";
        var startInfo = new ProcessStartInfo
        {
            FileName = "open",
            Arguments = shellArguments,
            UseShellExecute = true
        };

        try
        {
            return Process.Start(startInfo);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске процесса {filename}: {ex.Message}");
            return null;
        }
    }

    public override void StopProcess(Process? process)
    {
        if(process != null && !process.HasExited)
        {
            Console.WriteLine($"Остановка процесса с ID {process.Id}...");
            try
            {
                process.Kill();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Не удалось завершить процесс: {ex.Message}");
            }
        }
    }

    public override void CopyFile(string sourceFile, string outputDirectory)
    {
        // Используем команду `cp` для копирования файлов
        var arguments = $"-c cp \"{sourceFile}\" \"{outputDirectory}\"";
        StartServer("sh", arguments);
    }

    public override void MkDir(string outputDirectory)
    {
        // Используем команду `mkdir` для создания папки
        var arguments = $"-c mkdir -p \"{outputDirectory}\"";
        StartServer("sh", arguments);
    }

    public override void CopyResource(string resourceName, string folderPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));

        if(fullResourceName == null)
        {
            Console.WriteLine($"Ресурс '{resourceName}' не найден.");
            return;
        }

        Directory.CreateDirectory(folderPath);
        string destinationPath = Path.Combine(folderPath, resourceName);

        using(Stream? resourceStream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if(resourceStream == null)
            {
                Console.WriteLine($"Не удалось получить поток для ресурса '{fullResourceName}'.");
                return;
            }

            using(FileStream fileStream = File.Create(destinationPath))
            {
                resourceStream.CopyTo(fileStream);
            }
        }
        Console.WriteLine($"Ресурс '{resourceName}' успешно скопирован в '{destinationPath}'.");
    }

    public override void CopyResourceX(byte[] resource, string name, string folderPath)
    {
        Directory.CreateDirectory(folderPath);
        string destinationPath = Path.Combine(folderPath, name);
        File.WriteAllBytes(destinationPath, resource);
    }

    public override bool IsPortInUse(int port)
    {
        // На macOS эта часть кода работает так же, как и на Windows
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
        return tcpListeners.Any(endpoint => endpoint.Port == port);
    }

    public override bool IsJvmPlantUmlProcessRunning(string jarFilename)
    {
        // На macOS, как и на Linux, можно использовать команду `ps -ax`
        // или более удобную `pgrep` для поиска процессов.
        // Мы будем искать процесс `java` и проверять его аргументы.

        // `-f` ищет по полному имени и аргументам командной строки.
        // `-l` выводит имя процесса.
        var pgrep = new ProcessStartInfo
        {
            FileName = "pgrep",
            Arguments = $"-f \"{jarFilename}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using(var process = Process.Start(pgrep))
        {
            if(process == null) return false;

            // Читаем вывод команды `pgrep`
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Если вывод не пустой, значит, процесс найден
            return !string.IsNullOrEmpty(output);
        }
    }
}