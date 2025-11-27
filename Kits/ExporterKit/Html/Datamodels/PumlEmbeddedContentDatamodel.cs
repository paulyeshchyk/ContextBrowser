using System.IO;
using System.Net;
using ContextBrowserKit.Options.Export;
using ExporterKit.Html.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;

namespace ExporterKit.Html.Datamodels;

public abstract class PumlEmbeddedContentDatamodel<TTensor>
    where TTensor : notnull
{
    static readonly string SLocalHttpServerHost = "http://localhost:8081";
    static readonly string SLocalJavaServerHost = "http://localhost:8080";

    protected bool UseLocalFile { get; set; } = true;

    protected bool IsEmbedded { get; set; } = false;

    protected bool IsRelative { get; set; } = true;

    protected string ContentLocationFileSystemPath { get; set; } = "./../";

    protected string ContentLocationRemotePath { get; set; } = SLocalHttpServerHost;

    protected abstract string GetPumlFileName(TTensor contextKey);

    protected abstract string GetPumlFileName(string contextKey);

    public HtmlBuilder GetPumlBuilder(TTensor contextKey, ExportOptions exportOptions)
    {
        return (IsEmbedded)
            ? CreateEmbeddedPumlBuilder(contextKey, exportOptions)
            : CreateExternalPumlBuilder(contextKey, exportOptions);
    }

    public HtmlBuilder GetPumlBuilder(string contextKey, ExportOptions exportOptions)
    {
        return (IsEmbedded)
            ? CreateEmbeddedPumlBuilder(contextKey, exportOptions)
            : CreateExternalPumlBuilder(contextKey, exportOptions);
    }

    private string GetPath(bool isRelative, string fileName, ExportOptions exportOptions)
    {
        if (isRelative)
        {
            var pumlFilePath = exportOptions.FilePaths.BuildRelativePath(ExportPathType.puml, fileName);
            var host = UseLocalFile
                ? ContentLocationFileSystemPath
                : ContentLocationRemotePath;

            return Path.Combine(host, pumlFilePath);
        }
        else
        {
            return exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.puml, fileName);
        }
    }

    private HtmlBuilder CreateEmbeddedPumlBuilder(TTensor contextKey, ExportOptions exportOptions)
    {
        string fileName = GetPumlFileName(contextKey);
        var path = GetPath(IsRelative, fileName, exportOptions);
        string pumlFileContent = PumlInjector.GetPumlData(exportOptions, path);
        string encoded = WebUtility.HtmlEncode(pumlFileContent);
        return HtmlBuilderFactory.Puml(content: encoded, server: SLocalJavaServerHost);
    }

    private HtmlBuilder CreateEmbeddedPumlBuilder(string contextKey, ExportOptions exportOptions)
    {
        var fileName = GetPumlFileName(contextKey);
        var path = GetPath(IsRelative, fileName, exportOptions);
        string pumlFileContent = PumlInjector.GetPumlData(exportOptions, path);
        string encoded = WebUtility.HtmlEncode(pumlFileContent);
        return HtmlBuilderFactory.Puml(content: encoded, server: SLocalJavaServerHost);
    }

    private HtmlBuilder CreateExternalPumlBuilder(TTensor contextKey, ExportOptions exportOptions)
    {
        var fileName = GetPumlFileName(contextKey);
        var path = GetPath(IsRelative, fileName, exportOptions);
        return HtmlBuilderFactory.PumlReference(src: path, server: SLocalJavaServerHost);
    }

    private HtmlBuilder CreateExternalPumlBuilder(string contextKey, ExportOptions exportOptions)
    {
        var fileName = GetPumlFileName(contextKey);
        var path = GetPath(IsRelative, fileName, exportOptions);
        return HtmlBuilderFactory.PumlReference(src: path, server: SLocalJavaServerHost);
    }
}