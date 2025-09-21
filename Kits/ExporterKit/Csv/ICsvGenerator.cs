using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using HtmlKit.Options;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Csv;

public interface ICsvGenerator<TDataTensor>
    where TDataTensor : IDomainPerActionTensor
{
    void GenerateHeatmap(ITensorClassifierDomainPerActionContext contextClassifier, Dictionary<TDataTensor, List<object>> matrix, string outputPath, UnclassifiedPriorityType unclassifiedPriority = UnclassifiedPriorityType.None);
}
