﻿namespace ContextBrowserKit.Options.Export;

// context: settings, model
public record ExportOptions
{
    public ExportMatrixOptions ExportMatrix { get; set; }

    public ExportPaths FilePaths { get; set; }

    public ExportPaths WebPaths { get; set; }

    public ExportPumlOptions PumlOptions { get; }

    public ExportOptions(ExportMatrixOptions exportMatrix, ExportFilePaths filePaths, ExportWebPaths webPaths, ExportPumlOptions pumlOptions)
    {
        ExportMatrix = exportMatrix;
        FilePaths = filePaths;
        PumlOptions = pumlOptions;
        WebPaths = webPaths;
    }
}