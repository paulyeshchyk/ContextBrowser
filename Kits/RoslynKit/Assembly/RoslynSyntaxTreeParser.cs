using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

public class RoslynSyntaxTreeParser : ISyntaxTreeParser<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynSyntaxTreeParser(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

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

    public async Task<IEnumerable<RoslynSyntaxTreeWrapper>> ParseFilesToSyntaxTreesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        var treeCreationTasks = codeFiles.Select(async filePath =>
        {
            // Асинхронный I/O
            string code = await ReadAndInjectPseudoCodeAsync(options, filePath, cancellationToken).ConfigureAwait(false);

            // Синхронный CPU-bound парсинг
            var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: cancellationToken);
            return new RoslynSyntaxTreeWrapper(syntaxTree);
        }).ToList();

        return await Task.WhenAll(treeCreationTasks).ConfigureAwait(false);
    }
}