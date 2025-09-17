using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Compiler;

public interface IUmlDiagramCompiler
{
    Task<Dictionary<string, bool>> CompileAsync(CancellationToken cancellationToken);
}