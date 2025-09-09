using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ContextBrowserKit.Options.Export;
using ExporterKit;
using ExporterKit.Html.Puml;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using Microsoft.VisualBasic;

namespace ExporterKit.Html.Puml;

public static class PumlInjector
{
    private const string SPlantUmlScript = "\n<render-plantuml renderMode=\"svg\" src=\"{0}\" server=\"{1}\">\n{2}\n</render-plantuml>\n";

    // context: read, uml
    public static string ReadPumlContent(string pumlFilePath)
    {
        if (!File.Exists(pumlFilePath))
        {
            //throw new FileNotFoundException("Файл не найден", pumlFilePath);
            return string.Empty;
        }

        // Чтение всего файла в строку
        return File.ReadAllText(pumlFilePath);
    }

    public static string GetPumlData(ExportOptions exportOptions, string pumlFilePath)
    {
        return exportOptions.PumlOptions.InjectionType switch
        {
            PumlInjectionType.include => throw new NotImplementedException(),
            PumlInjectionType.inject => ReadPumlContent(pumlFilePath),
            _ => string.Empty
        };
    }

    private static string GetPumlContent(ExportOptions exportOptions, string pumlFilePath)
    {
        var src = exportOptions.PumlOptions.InjectionType switch
        {
            PumlInjectionType.include => $"{pumlFilePath}",
            PumlInjectionType.inject => string.Empty,
            _ => $"{pumlFilePath}"
        };

        var data = exportOptions.PumlOptions.InjectionType switch
        {
            PumlInjectionType.include => string.Empty,
            PumlInjectionType.inject => ReadPumlContent(pumlFilePath),
            _ => string.Empty
        };
        return string.Format(SPlantUmlScript, src, "http://localhost:8080", data);
    }
}