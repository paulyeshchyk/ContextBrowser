using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace ContextBrowser.Servers;

public class MacOsServer : CustomServer
{
    public override Process? StartShell(string arguments, bool autoclose = false)
    {
        var startInfo = MacOsProcessInfoFactory.ZSHProcessInfo(arguments);
        return StartServer(startInfo, arguments, (process, error1) =>
        {
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Output: {output}");
                    Console.WriteLine($"Error: {error}");
                }
            }
            else
            {
                Console.WriteLine(error1 != null ? $"Error: {error1}" : "Unknown error");
            }
        });
    }

    public override Process? StartServer(string filename, string arguments)
    {
        var shellArguments = $"-a Terminal {filename} --args {arguments}";
        var processInfo = MacOsProcessInfoFactory.OpenProcessInfo(shellArguments);
        var result = StartServer(processInfo, filename, (process, error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error: {error}");
            }
        });

        return result;
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
        StartShell($"cp \"{sourceFile}\" \"{outputDirectory}\"", true);
    }

    public override void MkDir(string outputDirectory)
    {
        StartShell($"mkdir -p \"{outputDirectory}\"", true);
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
        var pgrep = MacOsProcessInfoFactory.JvmIsRunningProcessInfo(jarFilename);
        using (var process = Process.Start(pgrep))
        {
            if (process == null) return false;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Если вывод не пустой, значит, процесс найден
            return !string.IsNullOrEmpty(output);
        }
    }

    public override Process? StartJar(string folder, string jarName, string args)
    {
        var processInfo = MacOsProcessInfoFactory.JavaRunJarProcessInfo(folder, jarName, args);
        return StartServer(processInfo, jarName, (process, error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error: {error}");
            }
        });
    }

    public override Process? StartHttpServer(int port, string folder)
    {
        // Запускаем 'http-server' напрямую
        var processInfo = MacOsProcessInfoFactory.HttpServerProcessInfo(port, folder);
        return StartServer(processInfo, folder, (process, error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error: {error}");
            }
        });
    }

    public override Process? OpenHtmlPage(string page)
    {
        var processInfo = MacOsProcessInfoFactory.OpenProcessInfo(page, true);
        return StartServer(processInfo, page, (process, error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error: {error}");
            }
        });
    }
}

public static class MacOsProcessInfoFactory
{
    public static ProcessStartInfo OpenProcessInfo(string shellArguments, bool createNoWindow = false)
    {
        return new ProcessStartInfo
        {
            FileName = "open",
            Arguments = shellArguments,
            UseShellExecute = true,
            CreateNoWindow = createNoWindow
        };
    }

    public static ProcessStartInfo ZSHProcessInfo(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/zsh",
            Arguments = $"-c \"{arguments}\"",
            UseShellExecute = false, // Не используем shell, чтобы контролировать процесс напрямую
            CreateNoWindow = true,   // Не создаём новое окно
            RedirectStandardOutput = true, // Перенаправляем вывод
            RedirectStandardError = true,  // Перенаправляем ошибки
        };
        return startInfo;
    }


    public static ProcessStartInfo HttpServerProcessInfo(int port, string folder)
    {
        //var http_module_path = "/usr/local/lib/node_modules/node/lib";
        var startInfo = new ProcessStartInfo
        {
            FileName = "npx",
            Arguments = $"http-server -p {port} --no-cache",
            WorkingDirectory = folder, // Важно: устанавливаем рабочую директорию
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        return startInfo;
    }

    public static ProcessStartInfo JavaRunJarProcessInfo(string folder, string jarName, string args)
    {
        return new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-jar \"{Path.Combine(folder, jarName)}\" {args}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
    }

    public static ProcessStartInfo JvmIsRunningProcessInfo(string jarFilename)
    {
        return new ProcessStartInfo
        {
            FileName = "pgrep",
            Arguments = $"-f \"{jarFilename}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }
}