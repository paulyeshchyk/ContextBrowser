using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, update
public class RoslynCodeInjector : ICodeInjector
{

    // context: roslyn, update
    public async Task<string> ReadAndInjectPseudoCodeAsync(SemanticOptions options, string filePath, CancellationToken cancellationToken)
    {
        var code = await File.ReadAllTextAsync(filePath, cancellationToken);
        if (!options.IncludePseudoCode)
        {
            return code;
        }

        if (!code.Contains("using System;", StringComparison.Ordinal))
        {
            // Вставим в самое начало, добавим отступ
            code = "using System;\n" + code;
        }
        return code;
    }
}
