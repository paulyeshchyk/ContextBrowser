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

public class RoslynCompilationMapMapper : ICompilationMapMapper<RoslynSyntaxTreeWrapper>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynCompilationMapMapper(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: roslyn, build, compilation

    public CompilationMap MapSemanticModelToSingleMap(ICompilationWrapper compilation, RoslynSyntaxTreeWrapper tree)
    {
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Compilation map building for: {tree.FilePath}");
        var model = compilation.GetSemanticModel(tree);
        var compilationMap = new CompilationMap(tree, model);
        return compilationMap;
    }

    // context: roslyn, build, compilation
    public SemanticCompilationMap MapSemanticModelToCompilationMap(IEnumerable<RoslynSyntaxTreeWrapper> syntaxTrees, ICompilationWrapper compilation)
    {
        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, "Compilation map building", LogLevelNode.Start);

        var compiledMaps = syntaxTrees
            .Select(tree => MapSemanticModelToSingleMap(compilation, tree))
            .ToList();

        var result = new SemanticCompilationMap(compiledMaps);

        _logger.WriteLog(AppLevel.R_Dll, LogLevel.Cntx, "Compilation map build done", LogLevelNode.End);
        return result;
    }
}
