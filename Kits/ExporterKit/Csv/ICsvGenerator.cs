using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model.Classifier;
using TensorKit.Model;

namespace ExporterKit.Csv;

public interface ICsvGenerator<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    void GenerateHeatmap(ITensorClassifierDomainPerActionContext contextClassifier, Dictionary<TDataTensor, List<object>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None);
}
