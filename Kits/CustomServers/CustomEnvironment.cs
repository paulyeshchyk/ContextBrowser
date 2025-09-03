using System;
using System.IO;
using System.Runtime.InteropServices;
using ContextBrowser.Servers;
using CustomServers;

namespace ContextBrowser.Services;

public static class CustomEnvironment
{
    private const string SPlantumlJarFilename = "plantuml-1.2025.4.jar";
    private const string SFaviconFilename = "favicon.ico";
    private const string SRenderPlantumlJsFilename = "render-plantuml.js";
    private const int SLocalHttpServerPort = 5500;
    private const string SPicowebJvmArgument = "-picoweb";

    private static CustomServer NewServer()
    {
        CustomServer customServer;
        var osPlatform = CustomServerDetector.GetOSPlatform();

        if (osPlatform == OSPlatform.Windows)
        {
            customServer = new WindowsServer();
        }
        else if (osPlatform == OSPlatform.OSX)
        {
            customServer = new MacOsServer();
        }
        else
        {
            throw new NotSupportedException("Операционная система не поддерживается.");
        }
        return customServer;
    }

    public static void CopyResources(string outputFolderPath)
    {
        var httpServerPath = Path.GetFullPath(outputFolderPath);
        var customServer = NewServer();
        customServer.MkDir(httpServerPath);
        customServer.CopyResourceX(Resources.favicon, SFaviconFilename, httpServerPath);
        customServer.CopyResourceX(Resources.plantuml_1_2025_4, SPlantumlJarFilename, httpServerPath);
        customServer.CopyResourceX(Resources.render_plantuml, SRenderPlantumlJsFilename, httpServerPath);
    }

    public static void RunServers(string outputFolderPath)
    {
        var httpServerPath = Path.GetFullPath(outputFolderPath);

        var customServer = NewServer();
        if (!customServer.IsPortInUse(SLocalHttpServerPort))
        {
            customServer.StartHttpServer(port: SLocalHttpServerPort, folder: httpServerPath);
            System.Threading.Thread.Sleep(1000);
        }

        if (!customServer.IsJvmPlantUmlProcessRunning(SPlantumlJarFilename))
        {
            customServer.StartJar(folder: httpServerPath, jarName: SPlantumlJarFilename, args: SPicowebJvmArgument);
            System.Threading.Thread.Sleep(1000);
        }

        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        customServer.OpenHtmlPage($"http://localhost:{SLocalHttpServerPort}/index.html?v={timestamp}");
    }
}