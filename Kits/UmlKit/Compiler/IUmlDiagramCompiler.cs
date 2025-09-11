using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Compiler;

public interface IUmlDiagramCompiler
{
    Dictionary<string, bool> Compile(IContextInfoDataset<ContextInfo> contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions);
}