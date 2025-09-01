using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.DiagramCompiler;

public interface IDiagramCompiler
{
    Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions);
}