using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using TensorKit.Model;

namespace ExporterKit.Csv;

public interface ICsvGenerator
{
    void GenerateHeatmap(IDomainPerActionContextTensorClassifier contextClassifier, Dictionary<DomainPerActionTensor, List<string>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None);
}
