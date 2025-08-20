using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ContextBrowser.Servers;

public abstract class CustomServer
{
    public abstract Process? StartServer(string filename, string arguments);

    public abstract Process? StartShell(string arguments, bool autoclose = false);

    public abstract Process? StartJar(string folder, string jarName, string args);

    public abstract Process? OpenHtmlPage(string page);

    public abstract Process? StartHttpServer(int port, string folder);

    public abstract void StopProcess(Process? process);

    public abstract void CopyFile(string sourceFile, string outputDirectory);

    public abstract void MkDir(string outputDirectory);

    public abstract void CopyResource(string resourceName, string folderPath);

    public abstract void CopyResourceX(byte[] resource, string name, string folderPath);

    public abstract bool IsPortInUse(int port);

    public abstract bool IsJvmPlantUmlProcessRunning(string jarFilename);

    public Process? StartServer(ProcessStartInfo startInfo, string? userInfo = null, Action<Process?, string?>? callback = null)
    {
        try
        {
            var result = Process.Start(startInfo);
            callback?.Invoke(result, null);
            return result;
        }
        catch(Exception ex)
        {
            callback?.Invoke(null, $"Ошибка при запуске процесса {userInfo ?? "unknown"}: {ex.Message}");
            return null;
        }
    }
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
        if(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return OSPlatform.FreeBSD;
        }
        throw new NotSupportedException("Операционная система не поддерживается.");
    }
}
