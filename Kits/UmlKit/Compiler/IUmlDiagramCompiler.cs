using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UmlKit.Compiler;

public interface IUmlDiagramCompiler
{
    Task<Dictionary<object, bool>> CompileAsync(CancellationToken cancellationToken);
}