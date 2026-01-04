using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;

namespace UmlKit.Compiler;

public interface IUmlDiagramCompiler
{
    Task<Dictionary<ILabeledValue, bool>> CompileAsync(CancellationToken cancellationToken);
}