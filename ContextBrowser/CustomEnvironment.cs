using System.Runtime.InteropServices;

namespace ContextBrowser.ContextCommentsParser;

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

        if(osPlatform == OSPlatform.Windows)
        {
            customServer = new WindowsServer();
        }
        else if(osPlatform == OSPlatform.OSX)
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
        if(!customServer.IsPortInUse(SLocalHttpServerPort))
            customServer.StartServer("cmd.exe", $"/c cd /d \"{httpServerPath}\" && start http-server -p {SLocalHttpServerPort}");
        if(!customServer.IsJvmPlantUmlProcessRunning(SPlantumlJarFilename))
            customServer.StartServer("cmd.exe", $"/c cd /d \"{httpServerPath}\" && start java -jar {SPlantumlJarFilename} {SPicowebJvmArgument}");

        System.Threading.Thread.Sleep(5000);

        customServer.StartServer("cmd.exe", $"/c start http://localhost:{SLocalHttpServerPort}/index.html");
    }
}