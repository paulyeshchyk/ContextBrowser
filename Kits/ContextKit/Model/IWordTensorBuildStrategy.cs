using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

public interface IWordTensorBuildStrategy<out TTensor>
    where TTensor : notnull
{
    int Priority { get; }
    IEnumerable<TTensor> BuildTensors(ContextElementGroups contextElementGroups);
    bool CanHandle(ContextElementGroups contextElementGroups, ExportMatrixOptions matrixOptions);
}
