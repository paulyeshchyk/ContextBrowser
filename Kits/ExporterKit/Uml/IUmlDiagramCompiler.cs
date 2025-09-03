using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Uml;
using UmlKit.Infrastructure.Options;

namespace ExporterKit.Uml;

public interface IUmlDiagramCompiler
{
    Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions);
}