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

// context: roslyn, build
public class RoslynSyntaxTreeParser : ISyntaxTreeParser<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ICodeInjector _codeInjector;

    public RoslynSyntaxTreeParser(IAppLogger<AppLevel> logger, ICodeInjector codeInjector)
    {
        _logger = logger;
        _codeInjector = codeInjector;
    }

    // context: roslyn, build
    public async Task<IEnumerable<RoslynSyntaxTreeWrapper>> ParseFilesToSyntaxTreesAsync(SemanticOptions options, IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Making syntax tries for {codeFiles.Count()} files", LogLevelNode.None);

        var treeCreationTasks = codeFiles.Select(async filePath =>
        {
            // Асинхронный I/O
            string code = await _codeInjector.ReadAndInjectPseudoCodeAsync(options, filePath, cancellationToken).ConfigureAwait(false);

            // Синхронный CPU-bound парсинг
            var syntaxTree = CSharpSyntaxTree.ParseText(code, path: filePath, cancellationToken: cancellationToken);
            return new RoslynSyntaxTreeWrapper(syntaxTree);
        }).ToList();

        return await Task.WhenAll(treeCreationTasks).ConfigureAwait(false);
    }
}