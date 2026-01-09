using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model.Options;

namespace SemanticKit.Parsers.File;

public interface IFileParserPipeline<TContext>
{
    Task<IEnumerable<TContext>> ParseAsync(string[] filePaths, string compilationName, CancellationToken ct);
}