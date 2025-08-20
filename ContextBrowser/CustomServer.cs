using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ContextBrowser.ContextCommentsParser;

public abstract class CustomServer
{
    public abstract Process? StartServer(string filename, string arguments);

    public abstract Process? StartShell(string arguments);

    public abstract Process? StartJar(string jarName, string folder, string args);

    public abstract Process? OpenHtmlPage(string page);

    public abstract Process? StartHttpServer(int port, string folder);

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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return OSPlatform.FreeBSD;
        }
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

        if (fullResourceName == null)
        {
            Console.WriteLine($"Ресурс '{resourceName}' не найден.");
            return;
        }

        Directory.CreateDirectory(folderPath);

        string destinationPath = Path.Combine(folderPath, resourceName);
        using (Stream? resourceStream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (resourceStream == null)
            {
                Console.WriteLine($"Не удалось получить поток для ресурса '{fullResourceName}'.");
                return;
            }

            using (FileStream fileStream = File.Create(destinationPath))
            {
                resourceStream.CopyTo(fileStream);
            }
        }

        Console.WriteLine($"Ресурс '{resourceName}' успешно скопирован в '{destinationPath}'.");
    }

    public override void CopyFile(string sourceFile, string outputDirectory)
    {
        string arguments = $"/c copy \"{sourceFile}\" \"{outputDirectory}\"";

        StartShell(arguments);
    }

    public override void MkDir(string outputDirectory)
    {
        string arguments = $"/c md \"{outputDirectory}\"";

        StartShell(arguments);
    }

    public override Process? StartShell(string arguments)
    {
        return StartServer("cmd.exe", arguments);
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
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске процесса {filename}: {ex.Message}");
            return null;
        }
    }

    public override void StopProcess(Process? process)
    {
        if (process != null && !process.HasExited)
        {
            Console.WriteLine($"Остановка процесса с ID {process.Id}...");
            try
            {
                process.Kill();
            }
            catch (Exception ex)
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

        foreach (var process in javaProcesses)
        {
            try
            {
                if (process.HasExited)
                    continue;

                string? executablePath = process.MainModule?.FileName;

                if (executablePath != null && executablePath.Contains(jarFilename))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки доступа, если нет прав
            }
        }
        return false;
    }

    public override Process? StartJar(string jarName, string folder, string args)
    {
        return StartShell($"/c cd /d \"{folder}\" && start java -jar {jarName} {args}");
    }

    public override Process? StartHttpServer(int port, string folder)
    {
        return StartShell($"/c cd /d \"{folder}\" && start http-server -p {port} --no-cache");
    }

    public override Process? OpenHtmlPage(string page)
    {
        return StartShell($"open {page}");
    }
}

public class MacOsServer : CustomServer
{

    public override Process? StartJar(string jarName, string folder, string args)
    {
        // Запускаем 'java' напрямую, указывая полный путь к файлу
        var startInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-jar \"{Path.Combine(folder, jarName)}\" {args}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        try { return Process.Start(startInfo); }
        catch (Exception ex) { Console.WriteLine($"Ошибка при запуске jar-файла: {ex.Message}"); return null; }
    }

    public override Process? StartHttpServer(int port, string folder)
    {
        // Запускаем 'http-server' напрямую
        var startInfo = new ProcessStartInfo
        {
            FileName = "http-server",
            Arguments = $"-p {port} --no-cache",
            WorkingDirectory = folder, // Важно: устанавливаем рабочую директорию
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        try { return Process.Start(startInfo); }
        catch (Exception ex) { Console.WriteLine($"Ошибка при запуске http-server: {ex.Message}"); return null; }

    }

    public override Process? OpenHtmlPage(string page)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "open",
            Arguments = page,
            UseShellExecute = true
        };

        try
        {
            return Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при открытии страницы {page}: {ex.Message}");
            return null;
        }
    }

    private Process? RunInTerminal(string command)
    {
        // Заменим " на ' внутри команды, чтобы не путаться с AppleScript кавычками
        var safeCommand = command.Replace("\"", "'");

        // AppleScript, который запустит команду в новом окне/табе Terminal
        var script = $"tell application \"Terminal\" to do script \"{safeCommand}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = "osascript",
            Arguments = $"-e \"{script}\"",
            UseShellExecute = true
        };

        try
        {
            return Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске команды в терминале: {ex.Message}");
            return null;
        }
    }

    public override Process? StartShell(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/zsh", // Или /bin/bash, в зависимости от предпочтений
            Arguments = $"-c \"{arguments}\"",
            // Важные изменения:
            UseShellExecute = false, // Не используем shell, чтобы контролировать процесс напрямую
            CreateNoWindow = true,   // Не создаём новое окно
            RedirectStandardOutput = true, // Перенаправляем вывод
            RedirectStandardError = true,  // Перенаправляем ошибки
        };

        try
        {
            // Запускаем процесс и сразу возвращаем его.
            var process = Process.Start(startInfo);
            if (process != null)
            {
                // Здесь можно прочитать вывод, чтобы увидеть ошибки
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Output: {output}");
                    Console.WriteLine($"Error: {error}");
                }
            }
            return process;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске shell-команды: {ex.Message}");
            return null;
        }
    }
    public override Process? StartServer(string filename, string arguments)
    {
        var shellArguments = $"-a Terminal {filename} --args {arguments}";
        var startInfo = new ProcessStartInfo
        {
            FileName = "open",
            Arguments = shellArguments,
            UseShellExecute = true,
            CreateNoWindow = false
        };
        try { return Process.Start(startInfo); }
        catch (Exception ex) { Console.WriteLine($"Ошибка при запуске процесса {filename}: {ex.Message}"); return null; }
    }

    public override void StopProcess(Process? process)
    {
        if (process != null && !process.HasExited)
        {
            Console.WriteLine($"Остановка процесса с ID {process.Id}...");
            try
            {
                process.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось завершить процесс: {ex.Message}");
            }
        }
    }

    public override void CopyFile(string sourceFile, string outputDirectory)
    {
        StartShell($"cp \"{sourceFile}\" \"{outputDirectory}\"");
    }

    public override void MkDir(string outputDirectory)
    {
        StartShell($"mkdir -p \"{outputDirectory}\"");
    }

    public override void CopyResource(string resourceName, string folderPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));

        if (fullResourceName == null)
        {
            Console.WriteLine($"Ресурс '{resourceName}' не найден.");
            return;
        }

        Directory.CreateDirectory(folderPath);
        string destinationPath = Path.Combine(folderPath, resourceName);

        using (Stream? resourceStream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (resourceStream == null)
            {
                Console.WriteLine($"Не удалось получить поток для ресурса '{fullResourceName}'.");
                return;
            }

            using (FileStream fileStream = File.Create(destinationPath))
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
        var pgrep = new ProcessStartInfo
        {
            FileName = "pgrep",
            Arguments = $"-f \"{jarFilename}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(pgrep))
        {
            if (process == null) return false;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Если вывод не пустой, значит, процесс найден
            return !string.IsNullOrEmpty(output);
        }
    }
}