using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace ContextBrowser.Servers;

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
        string arguments = $"copy \"{sourceFile}\" \"{outputDirectory}\"";

        StartShell(arguments, true);
    }

    public override void MkDir(string outputDirectory)
    {
        string arguments = $"md \"{outputDirectory}\"";

        StartShell(arguments, true);
    }

    public override Process? StartShell(string arguments, bool autoclose = false)
    {
        return StartServer("cmd.exe", autoclose ? $"/c {arguments}" : arguments);
    }

    public override Process? StartServer(string filename, string arguments)
    {
        var processInfo = WindowsProcessInfoFactory.CmdExeProcessInfo(filename, arguments);
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

    public override Process? StartJar(string folder, string jarName, string args)
    {
        return StartShell($"cd /d \"{folder}\" && start java -jar {jarName} {args}", true);
    }

    public override Process? StartHttpServer(int port, string folder)
    {
        return StartShell($"cd /d \"{folder}\" && start http-server -p {port} --no-cache", true);
    }

    public override Process? OpenHtmlPage(string page)
    {
        return StartShell($"start {page}", true);
    }
}

public static class WindowsProcessInfoFactory
{
    public static ProcessStartInfo CmdExeProcessInfo(string filename, string arguments)
    {
        return new ProcessStartInfo
        {
            FileName = filename,
            Arguments = arguments,
            UseShellExecute = true
        };
    }
}